using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Users.Dtos;
using JiggswapApi.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Users.Queries
{
    public class UserSigninQuery
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }

    public class UserSigninQueryValidator : AbstractValidator<UserSigninQuery>
    {
        private readonly IJiggswapDb _db;

        public UserSigninQueryValidator(IJiggswapDb db)
        {
            _db = db;

            RuleFor(v => v.Username)
                .NotEmpty();

            RuleFor(v => v.Password)
                .NotEmpty()
                .MustAsync(BeValidPassword)
                .WithMessage("Wrong Username/Password.");
        }

        private async Task<bool> BeValidPassword(UserSigninQuery query, string password, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            var existingHash = await conn.QuerySingleOrDefaultAsync<string>(
                "select password_hash from users where username = @Username",
            new { query.Username }).ConfigureAwait(false);

            if (string.IsNullOrEmpty(existingHash))
            {
                return false;
            }

            return PasswordService.VerifyPassword(existingHash, query.Password);
        }
    }
}