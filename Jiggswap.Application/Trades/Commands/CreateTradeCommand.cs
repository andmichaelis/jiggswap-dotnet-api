using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Common.Validation;
using MediatR;

namespace Jiggswap.Application.Trades.Commands
{
    public class CreateTradeCommand : IRequest<Guid>
    {
        public Guid InitiatorPuzzleId { get; set; }

        public Guid RequestedPuzzleId { get; set; }
    }

    public class CreateTradeCommandValidator : AbstractValidator<CreateTradeCommand>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUser;

        public CreateTradeCommandValidator(IJiggswapDb db, ICurrentUserService currentUser, CurrentUserAddressValidator addressValidator)
        {
            _db = db;
            _currentUser = currentUser;
            RuleFor(v => v.InitiatorPuzzleId)
                .MustAsync(BeOwnedByCurrentUser)
                .WithMessage("You do not own that puzzle.");

            RuleFor(v => v.RequestedPuzzleId)
                .MustAsync(NotBeOwnedByCurrentUser)
                .WithMessage("You already own the requested puzzle.");

            RuleFor(_ => currentUser).SetValidator(addressValidator);
        }

        private async Task<bool> NotBeOwnedByCurrentUser(Guid id, CancellationToken cancel)
        {
            return !(await BeOwnedByCurrentUser(id, cancel).ConfigureAwait(false));
        }

        private async Task<bool> BeOwnedByCurrentUser(Guid id, CancellationToken cancel)
        {
            using var conn = _db.GetConnection();
            var ownerId = await conn.QuerySingleOrDefaultAsync<int>("select owner_id from puzzles where public_id = @id", new { id }).ConfigureAwait(false);
            return _currentUser.InternalUserId == ownerId;
        }
    }

    public class CreateTradeCommandHandler : IRequestHandler<CreateTradeCommand, Guid>
    {
        private readonly IJiggswapDb _db;

        public CreateTradeCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<Guid> Handle(CreateTradeCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            const string getUserIdSql = @"
                select owner_id
                from puzzles
                where public_id = @PuzzleId
            ";

            const string getInternalPuzzleIdSql = @"
                select id
                from puzzles
                where public_id = @PuzzleId
            ";

            var requestedPuzzleParam = new
            {
                PuzzleId = request.RequestedPuzzleId
            };

            var initiatorPuzzleParam = new
            {
                PuzzleId = request.InitiatorPuzzleId
            };

            var requestedUserId = await conn.QuerySingleAsync<int>(getUserIdSql, requestedPuzzleParam).ConfigureAwait(false);
            var initiatorUserId = await conn.QuerySingleAsync<int>(getUserIdSql, initiatorPuzzleParam).ConfigureAwait(false);

            var requestedPuzzleId = await conn.QuerySingleAsync<int>(getInternalPuzzleIdSql, requestedPuzzleParam).ConfigureAwait(false);
            var initiatorPuzzleId = await conn.QuerySingleAsync<int>(getInternalPuzzleIdSql, initiatorPuzzleParam).ConfigureAwait(false);

            var tradeId = await conn.QuerySingleAsync<Guid>(
                @"insert into trades
                    (
                        initiator_puzzle_id,
                        initiator_user_id,
                        requested_puzzle_id,
                        requested_user_id
                    ) values (
                        @InitiatorPuzzleId,
                        @InitiatorUserId,
                        @RequestedPuzzleId,
                        @RequestedUserId
                    )
                    returning public_id", new
                {
                    RequestedPuzzleId = requestedPuzzleId,
                    RequestedUserId = requestedUserId,
                    InitiatorPuzzleId = initiatorPuzzleId,
                    InitiatorUserId = initiatorUserId
                }).ConfigureAwait(false);

            return tradeId;
        }
    }
}