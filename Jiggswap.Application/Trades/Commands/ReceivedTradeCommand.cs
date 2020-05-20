using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Trades.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Trades.Commands
{
    public class ReceivedTradeCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }
    }

    public class ReceivedTradeCommandValidator : AbstractValidator<ReceivedTradeCommand>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public ReceivedTradeCommandValidator(IJiggswapDb db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;

            RuleFor(v => v.TradeId)
                .MustAsync(IncludeCurrentUser);
        }

        private async Task<bool> IncludeCurrentUser(Guid tradeId, CancellationToken arg2)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleOrDefaultAsync<bool>(@"
                select
                    case
                    when
                        initiator_user_id = @UserId
                     or requested_user_id = @UserId
                    then true else false end IncludesCurrentUser
                from trades where public_id = @TradeId and status = @Active",
                new
                {
                    UserId = _currentUser.InternalUserId,
                    TradeId = tradeId,
                    TradeStates.Active
                });
        }
    }

    public class ReceivedTradeCommandHandler : IRequestHandler<ReceivedTradeCommand, bool>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public ReceivedTradeCommandHandler(IJiggswapDb db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(ReceivedTradeCommand request, CancellationToken cancellationToken)
        {
            await SetPuzzleAsReceived(request.TradeId);

            bool completed = await IsTradeCompleted(request);

            if (completed)
            {
                await SwapPuzzleOwners(request);

                await SetTradeCompleted(request);
            }

            return true;
        }

        private async Task SetTradeCompleted(ReceivedTradeCommand request)
        {
            using var conn = _db.GetConnection();

            await conn.ExecuteAsync(@"
                update trades
                set status = @Completed
                where public_id = @TradeId",
                new
                {
                    TradeStates.Completed,
                    request.TradeId
                });
        }

        private async Task SwapPuzzleOwners(ReceivedTradeCommand request)
        {
            using var conn = _db.GetConnection();

            await conn.ExecuteAsync(@"
                update puzzles P
                set owner_id = T.initiator_user_id,
                    is_in_trade = false
                from trades T
                where P.id = T.requested_puzzle_id
                and T.public_id = @TradeId",
                new
                {
                    request.TradeId
                });

            await conn.ExecuteAsync(@"
                update puzzles P
                set owner_id = T.requested_user_id,
                    is_in_trade = false
                from trades T
                where P.id = T.initiator_puzzle_id
                and T.public_id = @TradeId",
                new
                {
                    request.TradeId
                });
        }

        private async Task<bool> IsTradeCompleted(ReceivedTradeCommand request)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<bool>(@"
                select case when
                    initiator_puzzle_status = @Received and
                    requested_puzzle_status = @Received
                then 1 else 0 end TradeCompleted
                from trades where public_id = @TradeId",
                new
                {
                    request.TradeId,
                    PuzzleShipmentStates.Received
                });
        }

        private async Task SetPuzzleAsReceived(Guid tradeId)
        {
            using var conn = _db.GetConnection();

            var isInitiator = await conn.QuerySingleAsync<bool>(@"
                select case when initiator_user_id = @UserId then true else false end IsInitiator
                from trades where public_id = @TradeId",
            new
            {
                UserId = _currentUser.InternalUserId,
                TradeId = tradeId
            });

            if (isInitiator)
            {
                await conn.QueryAsync(@"
                    update trades
                    set requested_puzzle_status = @Received
                    where public_id = @TradeId",
                    new
                    {
                        TradeId = tradeId,
                        PuzzleShipmentStates.Received
                    });
            }
            else
            {
                await conn.QueryAsync(@"
                    update trades
                    set initiator_puzzle_status = @Received
                    where public_id = @TradeId
                ",
                new
                {
                    TradeId = tradeId,
                    PuzzleShipmentStates.Received
                });
            }
        }
    }
}