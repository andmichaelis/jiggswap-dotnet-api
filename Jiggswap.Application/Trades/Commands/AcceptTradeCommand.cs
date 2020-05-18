using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Common.Validation;
using Jiggswap.Application.Trades.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Trades.Commands
{
    public class AcceptTradeCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }
    }

    public class AcceptTradeCommandValidator : AbstractValidator<AcceptTradeCommand>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public AcceptTradeCommandValidator(IJiggswapDb db, ICurrentUserService currentUser, CurrentUserAddressValidator addressValidator)
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

            var tradeState = await conn.QuerySingleOrDefaultAsync<string>(sql, new { id });

            return tradeState == "proposed";
        }

        private async Task<bool> BeSentToCurrentUser(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();

            const string sql = "select requested_user_id from trades where public_id = @id";

            var requestedUserId = await conn.QuerySingleOrDefaultAsync<int>(sql, new { id });

            return requestedUserId == _currentUser.InternalUserId;
        }
    }

    public class AcceptTradeCommandHandler : IRequestHandler<AcceptTradeCommand, bool>
    {
        private readonly IJiggswapDb _db;

        public AcceptTradeCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<bool> Handle(AcceptTradeCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            conn.Open();

            using var transaction = conn.BeginTransaction();

            // activate the trade
            var tradeId = await conn.QuerySingleAsync<int>("update trades set status = @Active where Public_Id = @TradeId returning id",
            new
            {
                TradeStates.Active,
                request.TradeId
            });

            // inactivate other trades containing these puzzles
            await conn.QueryAsync(@"
            with trade_info as
            (
	            select
		            initiator_puzzle_id,
		            requested_puzzle_id
	            from
		            trades t
	            where t.id = @TradeId
            ), trades_to_inactivate as
            (
	            select
		            id
	            from
		            trades t2
		            join trade_info ti
		            on
			            (
				               t2.initiator_puzzle_id = ti.initiator_puzzle_id
				            or t2.initiator_puzzle_id = ti.requested_puzzle_id
				            or t2.requested_puzzle_id = ti.initiator_puzzle_id
				            or t2.requested_puzzle_id = ti.requested_puzzle_id
			            )
		            where t2.id != @TradeId
            )
            update trades tu
            set status = @Inactive
            where tu.id in (select id from trades_to_inactivate)",
                new
                {
                    TradeId = tradeId,
                    TradeStates.Inactive
                }
            );

            // set puzzles is_in_trade = true
            await conn.QueryAsync(@"
                with trade_puzzles as
                (
	                select initiator_puzzle_id pid
	                from trades where id = @TradeId
	                union
	                select requested_puzzle_id pid
	                from trades where id = @TradeId
                )
                update puzzles
                set is_in_trade = true
                where id in (select pid from trade_puzzles )",
                new
                {
                    TradeId = tradeId
                });

            transaction.Commit();

            conn.Close();

            return true;
        }
    }
}