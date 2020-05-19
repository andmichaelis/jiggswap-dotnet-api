using Dapper;
using Jiggswap.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.Puzzles.Queries
{
    public class PuzzleListItem
    {
        public Guid PuzzleId { get; set; }

        public string Title { get; set; }

        public int? ImageId { get; set; }

        [JsonIgnore]
        public string TagValue { get; set; }

        public string NumPieces { get; set; }

        public string NumPiecesMissing { get; set; }

        public bool IsInTrade { get; set; }

        public IEnumerable<string> Tags => string.IsNullOrEmpty(TagValue) ? Array.Empty<string>() : TagValue.Split(",");
    }

    public class GetPuzzlesListQuery : IRequest<IEnumerable<PuzzleListItem>>
    {
        public string Owner { get; set; }

        public string IgnoreOwner { get; set; }

        public bool? IncludeActiveTrades { get; set; }
    }

    public class GetPuzzlesListQueryHandler : IRequestHandler<GetPuzzlesListQuery, IEnumerable<PuzzleListItem>>
    {
        private readonly IJiggswapDb _db;

        public GetPuzzlesListQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PuzzleListItem>> Handle(GetPuzzlesListQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            var sql = @"
                select
                    P.public_id as PuzzleId,
                    P.Title,
                    P.image_id ImageId,
                    P.tags TagValue,
                    P.num_pieces NumPieces,
                    P.num_pieces_missing NumPiecesMissing,
                    P.is_in_trade IsInTrade
                from Puzzles P";

            if (!string.IsNullOrEmpty(request.Owner))
            {
                sql += @"
                    join users O on P.owner_id = O.id and O.Username = @Owner";
            }

            if (!string.IsNullOrEmpty(request.IgnoreOwner))
            {
                sql += @"
                    join users I on P.owner_id != I.id and I.Username = @IgnoreOwner";
            }

            sql += BuildWhereClause(request);

            return await conn.QueryAsync<PuzzleListItem>(sql, request).ConfigureAwait(false);
        }

        public static string BuildWhereClause(GetPuzzlesListQuery request)
        {
            if (request.IncludeActiveTrades == true)
            {
                return "";
            }
            else
            {
                return @"
                    where P.is_in_trade = false";
            }
        }
    }
}