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

            // set trade active
            await conn.QueryAsync("update trades set status = @Active where Public_Id = @TradeId",
            new
            {
                TradeStates.Active,
                request.TradeId
            });

            // set trades to inactive if they contain either puzzle

            return true;
        }
    }
}