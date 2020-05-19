using Dapper;
using FluentValidation;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Queries
{
    public class PuzzleDetailsResponse
    {
        public Guid PuzzleId { get; set; }

        public Guid OwnerUserId { get; set; }

        public string OwnerUsername { get; set; }

        public string Title { get; set; }

        public int NumPieces { get; set; }

        public int NumPiecesMissing { get; set; }

        public string AdditionalNotes { get; set; }

        public string BrandName => "";

        public string PuzzleConditionName => "";

        public bool IsOwner { get; set; }

        public int? ImageId { get; set; }

        public string TagValue { get; set; }

        public bool IsInTrade { get; set; }

        public IEnumerable<string> Tags => string.IsNullOrEmpty(TagValue) ? Array.Empty<string>() : TagValue.Split(",");
    }

    public class GetPuzzleDetailsQuery : IRequest<PuzzleDetailsResponse>
    {
        public Guid PuzzleId { get; set; }
    }

    public class GetPuzzleDetailsQueryValidator : AbstractValidator<GetPuzzleDetailsQuery>
    {
        public GetPuzzleDetailsQueryValidator()
        {
            RuleFor(v => v.PuzzleId)
                .NotNull();
        }
    }

    public class GetPuzzleDetailsQueryHandler : IRequestHandler<GetPuzzleDetailsQuery, PuzzleDetailsResponse>
    {
        private readonly IJiggswapDb _db;
        private readonly ICurrentUserService _currentUserService;

        public GetPuzzleDetailsQueryHandler(IJiggswapDb db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<PuzzleDetailsResponse> Handle(GetPuzzleDetailsQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            const string sql = @"
                    select
                        P.Public_Id as PuzzleId,
                        P.Title,
                        P.Num_Pieces NumPieces,
                        P.Num_Pieces_Missing NumPiecesMissing,
                        P.Additional_Notes AdditionalNotes,
                        O.Public_Id OwnerUserId,
                        O.Username OwnerUsername,
                        P.Image_Id ImageId,
                        P.Tags TagValue,
                        P.is_in_trade IsInTrade
                    from
                        Puzzles P
                        join Users O
                            on O.Id = P.Owner_Id
                    where
                        P.Public_Id = @PuzzleId";

            var details = await conn.QuerySingleAsync<PuzzleDetailsResponse>(sql, new { request.PuzzleId }).ConfigureAwait(false);

            details.IsOwner = _currentUserService.Username == details.OwnerUsername;

            return details;
        }
    }
}