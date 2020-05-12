using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Passwords.Notifications;
using Jiggswap.Notifications.Common;
using MediatR;
using StackExchange.Redis;

namespace Jiggswap.Application.Passwords.Commands
{
    public class ForgotPasswordCommand : IRequest<string>
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        private readonly IJiggswapDb _db;

        public ForgotPasswordCommandValidator(IJiggswapDb db)
        {
            _db = db;

            RuleFor(v => v.Email)
                .MustAsync(BeValidEmail)
                .WithMessage("Email not found.");
        }

        private async Task<bool> BeValidEmail(string email, CancellationToken arg2)
        {
            using var conn = _db.GetConnection();
            return await conn.QuerySingleOrDefaultAsync<bool>("select 1 from users where email = @email", new { email }).ConfigureAwait(false);
        }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
    {
        private readonly IJiggswapDb _db;
        private readonly IConnectionMultiplexer _redis;

        public ForgotPasswordCommandHandler(IJiggswapDb db, IJiggswapRedisConnection redis)
        {
            _db = db;
            _redis = redis.Redis;
        }

        public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var redisDb = _redis.GetDatabase();

            await redisDb.KeyDeleteAsync($"Jiggswap.PasswordReset.{request.Email}");

            var newToken = Path.GetRandomFileName().Replace(".", "");

            await redisDb.StringSetAsync($"Jiggswap.PasswordReset.{request.Email}", newToken, expiry: TimeSpan.FromMinutes(5));

            return newToken;
        }
    }
}