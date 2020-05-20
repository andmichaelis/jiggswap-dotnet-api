using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Trades.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Trades.Commands
{
    public class ShippedTradeCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }
    }

    public class ShippedTradeCommandValidator : AbstractValidator<ShippedTradeCommand>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public ShippedTradeCommandValidator(IJiggswapDb db, ICurrentUserService currentUser)
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
                from trades where public_id = @TradeId",
                new
                {
                    UserId = _currentUser.InternalUserId,
                    TradeId = tradeId
                });
        }
    }

    public class ShippedTradeCommandHandler : IRequestHandler<ShippedTradeCommand, bool>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public ShippedTradeCommandHandler(IJiggswapDb db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(ShippedTradeCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var isInitiator = await conn.QuerySingleAsync<bool>(@"
                select case when initiator_user_id = @UserId then true else false end IsInitiator
                from trades where public_id = @TradeId",
                new
                {
                    UserId = _currentUser.InternalUserId,
                    request.TradeId
                });

            if (isInitiator)
            {
                await conn.QueryAsync(@"
                    update trades
                    set initiator_puzzle_status = @Shipped
                    where public_id = @TradeId",
                    new
                    {
                        request.TradeId,
                        PuzzleShipmentStates.Shipped
                    });
            }
            else
            {
                await conn.QueryAsync(@"
                    update trades
                    set requested_puzzle_status = @Shipped
                    where public_id = @TradeId
                ",
                new
                {
                    request.TradeId,
                    PuzzleShipmentStates.Shipped
                });
            }

            return true;
        }
    }
}