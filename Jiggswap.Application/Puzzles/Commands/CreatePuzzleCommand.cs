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
    public class CreatePuzzleCommand : PuzzleCommandBase, IRequest<Guid>
    {
    }

    public class CreatePuzzleCommandValidator : AbstractValidator<CreatePuzzleCommand>
    {
        public CreatePuzzleCommandValidator()
        {
            Include(new PuzzleCommandBaseValidator());
        }
    }

    public class CreatePuzzleCommandHandler : IRequestHandler<CreatePuzzleCommand, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IJiggswapDb _db;

        public CreatePuzzleCommandHandler(ICurrentUserService currentUserService, IJiggswapDb db)
        {
            _currentUserService = currentUserService;
            _db = db;
        }

        public async Task<Guid> Handle(CreatePuzzleCommand request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var puzzleId = await conn.QuerySingleAsync<Guid>(@"
                    insert into puzzles
                        (owner_id, title, brand, num_pieces, num_pieces_missing, tags, additional_notes)
                    values
                        (@CurrentUserId, @Title, @Brand, @NumPieces, @NumPiecesMissing, @Tags, @AdditionalNotes)
                    returning public_id",
                new
                {
                    CurrentUserId = _currentUserService.InternalUserId,
                    request.Title,
                    request.Brand,
                    request.NumPieces,
                    request.NumPiecesMissing,
                    request.Tags,
                    request.AdditionalNotes
                })
                .ConfigureAwait(false);

            return puzzleId;
        }
    }
}