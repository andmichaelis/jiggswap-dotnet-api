
using Dapper;
using Jiggswap.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
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

        public IEnumerable<string> Tags => string.IsNullOrEmpty(TagValue) ? Array.Empty<string>() : TagValue.Split(",");
    }

    public class GetPuzzlesListQuery : IRequest<IEnumerable<PuzzleListItem>>
    {
        public string Username { get; set; }
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

            var sqlBuilder = new SqlBuilder();

            var template = sqlBuilder.AddTemplate(@"
                select 
                    P.public_id as PuzzleId,
                    P.Title,
                    P.image_id ImageId,
                    P.tags TagValue
                from Puzzles P
                    /**join**/
                    /**where**/");

            if (!string.IsNullOrEmpty(request.Username))
            {
                sqlBuilder.Join("Users U on P.owner_id = U.id");
                sqlBuilder.Where("U.Username = @Username", new { request.Username });
            }

            Console.WriteLine(template.RawSql);

            return await conn.QueryAsync<PuzzleListItem>(template.RawSql, template.Parameters).ConfigureAwait(false);
        }
    }
}