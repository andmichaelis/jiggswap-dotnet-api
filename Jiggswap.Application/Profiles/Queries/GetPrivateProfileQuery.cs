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
        public int UserId { get; set; }
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
                Zip,
                I.image_url ImageCdnUrl
            from
                user_profiles UP
                left outer join Images I
                on I.id = UP.image_id
            where UP.user_id = @UserId";

            return await conn.QuerySingleOrDefaultAsync<PrivateProfileDto>(sql, new
            {
                request.UserId
            }) ?? new PrivateProfileDto();
        }
    }
}