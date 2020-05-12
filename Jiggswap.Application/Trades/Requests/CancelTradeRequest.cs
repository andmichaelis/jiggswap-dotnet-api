using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Trades.Dtos;

namespace Jiggswap.Application.Trades.Requests
{
    public class CancelTradeRequest
    {
        public Guid TradeId { get; set; }
    }

    public class CancelTradeRequestValidator : AbstractValidator<CancelTradeRequest>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public CancelTradeRequestValidator(IJiggswapDb db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;

            RuleFor(v => v.TradeId)
                .Cascade(CascadeMode.StopOnFirstFailure)

                .MustAsync(IncludeCurrentUser)
                .WithMessage("You are not involved with this trade.")

                .MustAsync(BeActiveOrProposedTrade)
                .WithMessage("That trade is not active.");

            // Also: Puzzle not yet "Sent" by other party.
        }

        private async Task<bool> IncludeCurrentUser(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            const string initiatorSql = "select initiator_user_id from trades where public_id = @id";
            const string requestedSql = "select requested_user_id from trades where public_id = @id";

            var initiatorUserId = await conn.QuerySingleOrDefaultAsync<int>(initiatorSql, new { id }).ConfigureAwait(false);
            var requestedUserId = await conn.QuerySingleOrDefaultAsync<int>(requestedSql, new { id }).ConfigureAwait(false);

            return _currentUser.InternalUserId == initiatorUserId || _currentUser.InternalUserId == requestedUserId;
        }

        private async Task<bool> BeActiveOrProposedTrade(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            const string sql = "select status from trades where public_id = @id";

            var currentStatus = await conn.QuerySingleOrDefaultAsync<string>(sql, new { id }).ConfigureAwait(false);

            return currentStatus == TradeStates.Active || currentStatus == TradeStates.Proposed;
        }
    }
}