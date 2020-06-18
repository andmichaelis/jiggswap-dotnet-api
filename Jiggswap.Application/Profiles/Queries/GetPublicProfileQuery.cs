using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Profiles.Dtos;
using MediatR;

namespace Jiggswap.Application.Profiles.Queries
{
    public class PublicProfileDto
    {
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string DisplayCity { get; set; }

        public bool IsCurrentUser { get; set; }

        public string ImageCdnUrl { get; set; }
    }

    public class GetPublicProfileQuery : IRequest<PublicProfileDto>
    {
        public string Username { get; set; }

        public GetPublicProfileQuery(string username)
        {
            Username = username;
        }
    }

    public class GetPublicProfileQueryHandler : IRequestHandler<GetPublicProfileQuery, PublicProfileDto>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUserService;

        public GetPublicProfileQueryHandler(IJiggswapDb db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<PublicProfileDto> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var data = await conn.QuerySingleOrDefaultAsync<PrivateProfileDto>(@"
                select
                    UP.FirstName,
                    UP.LastName,
                    UP.StreetAddress,
                    UP.City,
                    UP.State,
                    UP.Zip,
                    I.image_url ImageCdnUrl
                from
                    user_profiles UP
                    join users U
                    on U.Id = UP.User_Id
                    left outer join images I
                    on I.Id = UP.image_id
                where
                    U.username = @Username",
                new
                {
                    request.Username
                }).ConfigureAwait(false) ?? new PrivateProfileDto();

            return new PublicProfileDto
            {
                Username = request.Username,
                IsCurrentUser = request.Username == _currentUserService.Username,
                DisplayName = GetDisplayName(data),
                DisplayCity = GetDisplayCity(data),
                ImageCdnUrl = data.ImageCdnUrl
            };
        }

        private static string GetDisplayName(PrivateProfileDto profile)
        {
            var firstName = profile.FirstName;
            var lastName = profile.LastName;

            var names = new List<string>();

            if (!string.IsNullOrEmpty(firstName))
            {
                names.Add(firstName);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                names.Add(lastName[0] + ".");
            }

            return string.Join(" ", names);
        }

        private static string GetDisplayCity(PrivateProfileDto profile)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(profile.City))
            {
                parts.Add(profile.City);
            }

            if (!string.IsNullOrEmpty(profile.State))
            {
                parts.Add(profile.State);
            }

            return string.Join(", ", parts);
        }
    }
}