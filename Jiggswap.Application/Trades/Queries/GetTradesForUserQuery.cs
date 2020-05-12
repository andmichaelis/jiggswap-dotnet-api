using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Domain.Trades;
using MediatR;

namespace Jiggswap.Application.Trades.Queries
{
    public class TradesForUserResponse
    {
        public IEnumerable<TradeDetailsDto> Trades { get; set; }
    }

    public class GetTradesForUserQuery : IRequest<TradesForUserResponse>
    {
        public int InternalUserId { get; set; }
    }

    public class GetTradesForUserQueryHandler : IRequestHandler<GetTradesForUserQuery, TradesForUserResponse>
    {
        private readonly IJiggswapDb _db;

        public GetTradesForUserQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<TradesForUserResponse> Handle(GetTradesForUserQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();
            var trades = await conn.QueryAsync<TradeDetailsDto>(@"
                select
                    T.public_id TradeId,
                    T.id TradeInternalId,
                    T.updated_at UpdatedAt,
                    T.Status,
                    IU.username InitiatorUsername,
                    RU.username RequestedUsername,
                    IP.title InitiatorPuzzleTitle,
                    RP.title RequestedPuzzleTitle,
                    IP.tags InitiatorPuzzleTags,
                    RP.tags RequestedPuzzleTags,
                    IP.image_id InitiatorPuzzleImageId,
                    RP.image_id RequestedPuzzleImageId
                from
                    Trades T
                    join users IU on IU.id = T.initiator_user_id
                    join users RU on RU.id = T.requested_user_id
                    join puzzles IP on IP.id = T.initiator_puzzle_id
                    join puzzles RP on RP.id = T.requested_puzzle_id
                where
                    T.initiator_user_id = @UserId or T.requested_user_id = @UserId",
                new
                {
                    UserId = request.InternalUserId
                }).ConfigureAwait(false);

            return new TradesForUserResponse
            {
                Trades = trades
            };
        }
    }
}