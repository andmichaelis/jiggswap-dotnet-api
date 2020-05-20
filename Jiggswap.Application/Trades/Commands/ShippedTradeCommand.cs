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

        public string Carrier { get; set; }

        public string TrackingNumber { get; set; }
    }

    public class ShippedTradeCommandValidator : AbstractValidator<ShippedTradeCommand>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public ShippedTradeCommandValidator(IJiggswapDb db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;

            RuleFor(v => v.Carrier)
                .NotEmpty()
                .WithMessage("Please enter which Shipping Carrier you used.");

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
                    set
                        initiator_puzzle_status = @Shipped,
                        initiator_puzzle_shipped_via = @Carrier,
                        initiator_puzzle_shipped_trackingno = @TrackingNumber
                    where public_id = @TradeId",
                    new
                    {
                        PuzzleShipmentStates.Shipped,
                        request.Carrier,
                        request.TrackingNumber,
                        request.TradeId,
                    });
            }
            else
            {
                await conn.QueryAsync(@"
                    update trades
                    set
                        requested_puzzle_status = @Shipped,
                        requested_puzzle_shipped_via = @Carrier,
                        requested_puzzle_shipped_trackingno = @TrackingNumber
                    where public_id = @TradeId
                ",
                new
                {
                    PuzzleShipmentStates.Shipped,
                    request.Carrier,
                    request.TrackingNumber,
                    request.TradeId,
                });
            }

            return true;
        }
    }
}