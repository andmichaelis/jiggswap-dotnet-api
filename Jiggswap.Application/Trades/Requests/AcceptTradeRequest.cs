using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using FluentValidation.Internal;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Common.Validation;
using Jiggswap.Application.Profiles.Dtos;
using Jiggswap.Application.Profiles.Queries;

namespace Jiggswap.Application.Trades.Requests
{
    public class AcceptTradeRequest
    {
        public Guid TradeId { get; set; }
    }

    public class AcceptTradeRequestValidator : AbstractValidator<AcceptTradeRequest>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public AcceptTradeRequestValidator(IJiggswapDb db, ICurrentUserService currentUser, CurrentUserAddressValidator addressValidator)
        {
            _db = db;
            _currentUser = currentUser;

            RuleFor(t => t.TradeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()

                .MustAsync(BeProposedTrade)
                .WithMessage("This trade is not currently proposed.")

                .MustAsync(BeSentToCurrentUser)
                .WithMessage("You are not the recipient of this trade.");

            RuleFor(_ => currentUser).SetValidator(addressValidator);
        }

        private async Task<bool> BeProposedTrade(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            const string sql = "select status from trades where public_id = @id";

            var tradeState = await conn.QuerySingleOrDefaultAsync<string>(sql, new { id }).ConfigureAwait(false);

            return tradeState == "proposed";
        }

        private async Task<bool> BeSentToCurrentUser(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            const string sql = "select requested_user_id from trades where public_id = @id";

            var requestedUserId = await conn.QuerySingleOrDefaultAsync<int>(sql, new { id }).ConfigureAwait(false);

            return requestedUserId == _currentUser.InternalUserId;
        }
    }
}