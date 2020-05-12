using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Commands
{
    public class UpdatePuzzleCommand : PuzzleCommandBase, IRequest<Guid>
    {
        public Guid PuzzleId { get; set; }
    }

    public class UpdatePuzzleCommandValidator : AbstractValidator<UpdatePuzzleCommand>
    {
        public UpdatePuzzleCommandValidator(IJiggswapDb db, ICurrentUserService currentUserService)
        {
            RuleFor(v => v.PuzzleId)
                .Must((_, puzzleId) => BeOwnedByCurrentUser(puzzleId, db, currentUserService.InternalUserId))
                .WithMessage("This puzzle is not owned by you.");

            Include(new PuzzleCommandBaseValidator());
        }

        private bool BeOwnedByCurrentUser(Guid puzzleId, IJiggswapDb db, int currentUserId)
        {
            using var conn = db.GetConnection();

            var puzzleOwnerId = conn.QuerySingleOrDefault<int>(
                @"select 
                    p.owner_id
                from 
                    puzzles p
                where 
                    p.public_id = @PuzzleId", new
                {
                    PuzzleId = puzzleId
                });

            return puzzleOwnerId == currentUserId;
        }
    }

    public class UpdatePuzzleCommandHandler : IRequestHandler<UpdatePuzzleCommand, Guid>
    {
        private readonly IJiggswapDb _db;

        public UpdatePuzzleCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<Guid> Handle(UpdatePuzzleCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            const string sql = @"
                update puzzles set
                    title = @Title,
                    num_pieces = @NumPieces,
                    num_pieces_missing = @NumPiecesMissing,
                    additional_notes = @AdditionalNotes,
                    tags = @Tags
                where
                    public_id = @PuzzleId";

            await conn.ExecuteAsync(sql, new
            {
                request.Title,
                request.NumPieces,
                request.NumPiecesMissing,
                request.AdditionalNotes,
                request.Tags,
                request.PuzzleId
            }).ConfigureAwait(false);

            return request.PuzzleId;
        }
    }
}