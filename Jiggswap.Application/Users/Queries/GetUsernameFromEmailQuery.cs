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
    public class GetUsernameFromEmailQuery : IRequest<string>
    {
        public GetUsernameFromEmailQuery(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }

    public class GetUsernameFromEmailQueryHandler : IRequestHandler<GetUsernameFromEmailQuery, string>
    {
        private readonly IJiggswapDb _db;

        public GetUsernameFromEmailQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<string> Handle(GetUsernameFromEmailQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleOrDefaultAsync<string>("select username from users where email = @Email", new { request.Email });
        }
    }
}