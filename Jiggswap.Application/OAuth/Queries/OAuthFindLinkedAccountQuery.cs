using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.OAuth.Dtos;
using Jiggswap.Application.Users.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.OAuth.Queries
{
    public class OAuthFindLinkedAccountQuery : IRequest<AuthorizedUserResponse>
    {
        public OAuthService Service { get; }
        public string ServiceUserId { get; }

        public OAuthFindLinkedAccountQuery(OAuthService service, string serviceUserId)
        {
            Service = service;
            ServiceUserId = serviceUserId;
        }
    }

    public class OAuthFindLinkedAccountQueryHandler : IRequestHandler<OAuthFindLinkedAccountQuery, AuthorizedUserResponse>
    {
        private IJiggswapDb _db;

        public OAuthFindLinkedAccountQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<AuthorizedUserResponse> Handle(OAuthFindLinkedAccountQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var username = await conn.QuerySingleOrDefaultAsync<string>(@"
                select
                    U.username
                from
                    user_oauth_data UOD
                    join users U
                        on UOD.jiggswap_user_id = U.id
                where
                    UOD.Service = @Service
                    and UOD.service_user_id = @ServiceUserId",
                    new
                    {
                        Service = request.Service.ToString(),
                        request.ServiceUserId
                    });

            return new AuthorizedUserResponse
            {
                Username = username
            };
        }
    }
}