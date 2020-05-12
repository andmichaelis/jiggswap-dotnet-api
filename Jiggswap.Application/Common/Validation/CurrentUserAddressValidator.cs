using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Profiles.Dtos;

namespace Jiggswap.Application.Common.Validation
{
    public class CurrentUserAddressValidator : AbstractValidator<ICurrentUserService>
    {
        private readonly IJiggswapDb _db;

        public CurrentUserAddressValidator(IJiggswapDb db)
        {
            _db = db;

            RuleFor(v => v.InternalUserId)
                .MustAsync(BeValidAddress)
                .WithMessage("You need to have a valid address.");
        }

        private async Task<bool> BeValidAddress(int id, CancellationToken cancellation)
        {
            using var conn = _db.GetConnection();
            var address = await conn.QuerySingleOrDefaultAsync<Address>(@"
                select
                    StreetAddress,
                    City,
                    State,
                    Zip
                from
                    user_profiles
                where user_id = @id", new { id }).ConfigureAwait(false);

            var validator = new AddressValidator();

            return validator.Validate(address).IsValid;
        }
    }
}