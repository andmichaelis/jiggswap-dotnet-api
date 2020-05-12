using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Profiles.Dtos;
using MediatR;

namespace Jiggswap.Application.Profiles.Commands
{
    public class SaveProfileCommand : PrivateProfileDto, IRequest<bool> { }

    public class SaveProfileCommandValidator : AbstractValidator<SaveProfileCommand>
    {
        public SaveProfileCommandValidator(AddressValidator addressValidator)
        {
            RuleFor(v => v).SetValidator(addressValidator);
        }
    }

    public class SaveProfileCommandHandler : IRequestHandler<SaveProfileCommand, bool>
    {
        private readonly IJiggswapDb _db;
        private readonly int _currentUserId;

        public SaveProfileCommandHandler(IJiggswapDb db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserId = currentUserService.InternalUserId;
        }

        public async Task<bool> Handle(SaveProfileCommand command, CancellationToken cancellationToken)
        {
            var profileId = await GetOrCreateProfile(_currentUserId).ConfigureAwait(false);

            return await UpdateProfile(profileId, command).ConfigureAwait(false);
        }

        private async Task<bool> UpdateProfile(int profileId, SaveProfileCommand command)
        {
            using var conn = _db.GetConnection();

            const string updateSql = @"
                update user_profiles 
                set 
                    FirstName = @FirstName,
                    LastName = @LastName,
                    StreetAddress = @StreetAddress,
                    City = @City,
                    State = @State,
                    Zip = @Zip
                where
                    id = @ProfileId";

            await conn.ExecuteAsync(updateSql, new
            {
                command.FirstName,
                command.LastName,
                command.StreetAddress,
                command.City,
                command.State,
                command.Zip,
                ProfileId = profileId
            }).ConfigureAwait(false);

            return true;
        }

        private async Task<int> GetOrCreateProfile(int currentUserId)
        {
            using var conn = _db.GetConnection();

            const string getSql = "select id from user_profiles where user_id = @CurrentUserId";
            const string createSql = "insert into user_profiles (user_id) values (@CurrentUserId) returning id";

            var idParam = new
            {
                CurrentUserId = currentUserId
            };

            var profileId = await conn.QuerySingleOrDefaultAsync<int>(getSql, idParam).ConfigureAwait(false);

            if (profileId != 0)
            {
                return profileId;
            }

            return await conn.QuerySingleAsync<int>(createSql, idParam).ConfigureAwait(false);
        }
    }
}
