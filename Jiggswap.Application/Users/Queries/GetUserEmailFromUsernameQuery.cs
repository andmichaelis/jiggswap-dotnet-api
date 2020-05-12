using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using MediatR;

namespace Jiggswap.Application.Users.Queries
{
    public class GetUserEmailFromUsernameQuery : IRequest<string>
    {
        public string Username { get; set; }

        public GetUserEmailFromUsernameQuery(string username)
        {
            Username = username;
        }
    }

    public class GetUserEmailFromUsernameQueryHandler : IRequestHandler<GetUserEmailFromUsernameQuery, string>
    {
        private readonly IJiggswapDb _db;

        public GetUserEmailFromUsernameQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<string> Handle(GetUserEmailFromUsernameQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<string>(
                "select email from users where username = @Username",
                new { request.Username }).ConfigureAwait(false);
        }
    }
}