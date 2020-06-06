using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.OAuth.Queries
{
    public class OAuthSignupQuery
    {
        public string Token { get; set; }

        public string Username { get; set; }
    }

    public class OAuthSignupQueryValidator : AbstractValidator<OAuthSignupQuery>
    {
        private readonly IJiggswapDb _db;

        public OAuthSignupQueryValidator(IJiggswapDb db)
        {
            _db = db;

            RuleFor(vm => vm.Username)
                .NotNull()
                .Length(6, 50)
                .Must(NotHaveInvalidCharacters)
                .WithMessage("Username can only contain letters and numbers.")
                .MustAsync(BeUniqueUsername)
                .WithMessage("Username is already taken.");
        }

        private bool NotHaveInvalidCharacters(string arg)
        {
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            return r.IsMatch(arg);
        }

        private async Task<bool> BeUniqueUsername(string username, CancellationToken arg2)
        {
            using var conn = _db.GetConnection();

            var isTaken = await conn.QuerySingleAsync<bool>("select case when exists (select id from users where username = @Username) then true else false end", new { username });

            return !isTaken;
        }
    }
}