using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.Profiles.Dtos;
using MediatR;

namespace Jiggswap.Application.Profiles.Queries
{
    public class GetPrivateProfileQuery : IRequest<PrivateProfileDto>
    {
        public string Username { get; set; }

        public GetPrivateProfileQuery(string username)
        {
            Username = username;
        }
    }

    public class GetPrivateProfileQueryHandler : IRequestHandler<GetPrivateProfileQuery, PrivateProfileDto>
    {
        private readonly IJiggswapDb _db;

        public GetPrivateProfileQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<PrivateProfileDto> Handle(GetPrivateProfileQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            const string sql = @"select
                FirstName,
                LastName,
                StreetAddress,
                City,
                State,
                Zip
            from
                user_profiles UP
                join users U
                on U.id = UP.user_id
            where U.username = @Username";

            return await conn.QuerySingleOrDefaultAsync<PrivateProfileDto>(sql, new
            {
                request.Username
            }).ConfigureAwait(false) ?? new PrivateProfileDto();
        }
    }
}
