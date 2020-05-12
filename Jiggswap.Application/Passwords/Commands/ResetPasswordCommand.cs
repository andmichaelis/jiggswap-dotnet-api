using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Validation;
using Jiggswap.Application.Passwords.Dtos;
using Jiggswap.Application.Users.Dtos;
using JiggswapApi.Services;
using MediatR;
using StackExchange.Redis;

namespace Jiggswap.Application.Passwords.Commands
{
    public class ResetPasswordCommand : IPasswordWithConfirmation, IRequest<AuthorizedUserResponse>
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }

    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly IConnectionMultiplexer _redis;

        public ResetPasswordCommandValidator(IJiggswapRedisConnection redis)
        {
            _redis = redis.Redis;

            RuleFor(v => v)
                .MustAsync(BeValidTokenForEmail)
                .WithMessage("Provided token is not valid.");

            Include(new PasswordValidator());
        }

        private async Task<bool> BeValidTokenForEmail(ResetPasswordCommand command, CancellationToken arg2)
        {
            var db = _redis.GetDatabase();

            var storedToken = await db.StringGetAsync($"Jiggswap.PasswordReset.{command.Email}");

            return storedToken == command.Token;
        }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, AuthorizedUserResponse>
    {
        private readonly IJiggswapDb _db;
        private readonly IConnectionMultiplexer _redis;

        public ResetPasswordCommandHandler(IJiggswapDb db, IJiggswapRedisConnection redis)
        {
            _db = db;
            _redis = redis.Redis;
        }

        public async Task<AuthorizedUserResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();
            var redisConn = _redis.GetDatabase();

            var userId = await GetUserIdFromEmail(request.Email).ConfigureAwait(false);

            var usernameTask = GetUsernameFromEmail(request.Email);

            await SaveNewPassword(userId, request.Password).ConfigureAwait(false);

            redisConn.KeyDelete($"Jiggswap.PasswordReset.{request.Email}");

            var username = await usernameTask;

            return new AuthorizedUserResponse { Username = username };
        }

        private async Task<int> GetUserIdFromEmail(string email)
        {
            using var conn = _db.GetConnection();
            return await conn.QuerySingleAsync<int>("select id from users where email = @email", new { email }).ConfigureAwait(false);
        }

        private async Task<string> GetUsernameFromEmail(string email)
        {
            using var conn = _db.GetConnection();
            return await conn.QuerySingleAsync<string>("select username from users where email = @email", new { email });
        }

        private async Task SaveNewPassword(int userId, string password)
        {
            var hash = PasswordService.HashPassword(password);

            using var conn = _db.GetConnection();
            await conn.ExecuteAsync("update users set password_hash = @hash where id = @userId", new { hash, userId }).ConfigureAwait(false);
        }
    }
}