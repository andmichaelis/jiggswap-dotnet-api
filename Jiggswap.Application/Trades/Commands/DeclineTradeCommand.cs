﻿using Dapper;
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
    public class DeclineTradeCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }
    }

    public class DeclineTradeCommandValidator : AbstractValidator<DeclineTradeCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IJiggswapDb _db;

        public DeclineTradeCommandValidator(ICurrentUserService currentUser, IJiggswapDb db)
        {
            _currentUser = currentUser;
            _db = db;

            RuleFor(t => t.TradeId)
                .MustAsync(BeSentToCurrentUser)
                .WithMessage("You are not a member of this trade or it is not longer proposed");
        }

        private async Task<bool> BeSentToCurrentUser(Guid tradeId, CancellationToken arg2)
        {
            using var conn = _db.GetConnection();

            var tradeUser = await conn.QuerySingleOrDefaultAsync<int>(@"
                select
                    T.requested_user_id
                from trades T
                where T.public_id = @TradeId and T.status = @Proposed",
                new
                {
                    TradeId = tradeId,
                    TradeStates.Proposed
                });

            return tradeUser == _currentUser.InternalUserId;
        }
    }

    public class DeclineTradeCommandHandler : IRequestHandler<DeclineTradeCommand, bool>
    {
        private readonly IJiggswapDb _db;

        public DeclineTradeCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<bool> Handle(DeclineTradeCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            await conn.ExecuteAsync(@"
                update
                    trades
                set
                    status = @Inactive
                where
                    public_id = @TradeId",
                 new
                 {
                     TradeStates.Inactive,
                     request.TradeId
                 });

            return true;
        }
    }
}