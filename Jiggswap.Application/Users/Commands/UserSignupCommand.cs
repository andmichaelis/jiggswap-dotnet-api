using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Validation;
using Jiggswap.Application.Users.Dtos;
using JiggswapApi.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Users.Commands
{
    public class UserSignupCommand : IPasswordWithConfirmation, IRequest<AuthorizedUserResponse>
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }

    public class UserSignupCommandHandler : IRequestHandler<UserSignupCommand, AuthorizedUserResponse>
    {
        private readonly IJiggswapDb _db;

        public UserSignupCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<AuthorizedUserResponse> Handle(UserSignupCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = PasswordService.HashPassword(request.Password);

            using var conn = _db.GetConnection();

            await conn.ExecuteAsync(@"
                insert into users
                    (username, email, password_hash)
                values
                    (@Username, @Email, @PasswordHash)",
                new
                {
                    request.Username,
                    request.Email,
                    PasswordHash = passwordHash
                }).ConfigureAwait(false);

            return new AuthorizedUserResponse
            {
                Username = request.Username
            };
        }
    }

    public class UserSignupCommandValidator : AbstractValidator<UserSignupCommand>
    {
        public UserSignupCommandValidator(IJiggswapDb db)
        {
            RuleFor(v => v.Username)
                .NotNull()
                .Length(6, 50)
                .Must(v => BeUniqueUsername(v, db))
                .WithMessage("{PropertyValue} is already taken.");

            RuleFor(v => v.Email)
                .NotNull()
                .Length(6, 50)
                .Must(v => BeUniqueEmail(v, db))
                .WithMessage("{PropertyValue} is already taken.");

            Include(new PasswordValidator());
        }

        private bool BeUniqueEmail(string email, IJiggswapDb db)
        {
            using var conn = db.GetConnection();

            var existingEmails = conn.QuerySingle<int>("select count(email) from users where email = @Email", new
            {
                Email = email
            });

            return existingEmails == 0;
        }

        private bool BeUniqueUsername(string username, IJiggswapDb db)
        {
            using var conn = db.GetConnection();

            var existingUsernames = conn.QuerySingle<int>("select count(username) from users where username = @Username", new
            {
                Username = username
            });

            return existingUsernames == 0;
        }
    }
}