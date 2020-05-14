using Jiggswap.Domain;
using Jiggswap.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Queries
{
    public class PuzzleListItemDto
    {
        public Guid PuzzleId { get; set; }

        public string Title { get; set; }

        public int? ImageId { get; set; }

        [JsonIgnore]
        public string TagValue { get; set; }

        public int NumPieces { get; set; }

        public int NumPiecesMissing { get; set; }

        public IEnumerable<string> Tags => string.IsNullOrEmpty(TagValue) ? Array.Empty<string>() : TagValue.Split(",");

        public PuzzleListItemDto(Puzzle from)
        {
            PuzzleId = from.PublicId;
            Title = from.Title;
            ImageId = from.ImageId;
            TagValue = from.Tags;
            NumPieces = from.NumPieces;
            NumPiecesMissing = from.NumPiecesMissing;
        }
    }

    public class GetPuzzlesListEfQuery : IRequest<IEnumerable<PuzzleListItemDto>>
    {
    }

    public class GetPuzzleListEfQueryHandler : IRequestHandler<GetPuzzlesListEfQuery, IEnumerable<PuzzleListItemDto>>
    {
        private readonly JiggswapEfContext _context;

        public GetPuzzleListEfQueryHandler(JiggswapEfContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PuzzleListItemDto>> Handle(GetPuzzlesListEfQuery request, CancellationToken cancellationToken)
        {
            return _context.Puzzles.Select(p => new PuzzleListItemDto(p));
        }
    }
}