using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.OAuth.Dtos;
using Jiggswap.Application.Users.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.OAuth.Commands
{
    public class OAuthUserSigninCommand : IRequest<AuthorizedUserResponse>
    {
        public OAuthUserData OAuthData { get; set; }
    }

    public class OAuthUserSigninCommandHandler : IRequestHandler<OAuthUserSigninCommand, AuthorizedUserResponse>
    {
        private readonly IJiggswapDb _db;
        private readonly ILogger<OAuthUserSigninCommandHandler> _logger;

        public OAuthUserSigninCommandHandler(IJiggswapDb db, ILogger<OAuthUserSigninCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        private async Task<int?> GetJiggswapUserId(OAuthService service, string serviceUserId)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleOrDefaultAsync<int?>(@"
                select od.jiggswap_user_id
                from user_oauth_data od
                where
                    od.service = @Service
                    and od.service_user_id = @ServiceUserId",
                new
                {
                    Service = service.ToString(),
                    serviceUserId
                });
        }

        private async Task<int> CreateJiggswapUser(OAuthUserData userData)
        {
            using var conn = _db.GetConnection();

            var username = userData.FirstName + userData.LastName[0];

            var email = userData.Email;

            var userId = await conn.QuerySingleAsync<int>(@"
                insert into users
                ( username, email, password_hash )
                values
                ( @Username, @Email, @PasswordHash )
                returning id",
                new
                {
                    username,
                    userData.Email,
                    PasswordHash = ""
                });

            await conn.ExecuteAsync(@"
                insert into user_profiles
                ( user_id, firstname, lastname )
                values
                ( @UserId, @FirstName, @LastName )",
                new
                {
                    userId,
                    userData.FirstName,
                    userData.LastName
                });

            await conn.ExecuteAsync(@"
                insert into user_oauth_data
                ( jiggswap_user_id, service, service_user_id )
                values
                ( @UserId, @Service, @ServiceUserId )",
                new
                {
                    userId,
                    Service = userData.Service.ToString(),
                    userData.ServiceUserId
                });

            return userId;
        }

        private async Task<string> GetUsername(int userId)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<string>("select username from users where id = @UserId", new { userId });
        }

        public async Task<AuthorizedUserResponse> Handle(OAuthUserSigninCommand request, CancellationToken cancellationToken)
        {
            var jiggswapUserId = await GetJiggswapUserId(request.OAuthData.Service, request.OAuthData.ServiceUserId);

            _logger.LogInformation("Existing User Id: {0}", jiggswapUserId);

            if (!jiggswapUserId.HasValue)
            {
                jiggswapUserId = await CreateJiggswapUser(request.OAuthData);

                _logger.LogInformation("Created user. New Id: {0}", jiggswapUserId);
            }

            var username = await GetUsername(jiggswapUserId.Value);

            return new AuthorizedUserResponse { Username = username };
        }
    }
}