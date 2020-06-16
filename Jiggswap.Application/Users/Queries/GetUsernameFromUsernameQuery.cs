using Dapper;
using Jiggswap.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Users.Queries
{
    public class GetUsernameFromUsernameQuery : IRequest<string>
    {
        public string Username { get; set; }

        public GetUsernameFromUsernameQuery(string username)
        {
            Username = username;
        }
    }

    public class GetUsernameFromUsernameQueryHandler : IRequestHandler<GetUsernameFromUsernameQuery, string>
    {
        private readonly IJiggswapDb _db;

        public GetUsernameFromUsernameQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<string> Handle(GetUsernameFromUsernameQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<string>(
                "select username from users where lower(username) = @Username",
                new { Username = request.Username.ToLower() });
        }
    }
}