using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.Trades.Dtos;
using Jiggswap.Domain.Trades;
using MediatR;

namespace Jiggswap.Application.Trades.Queries
{
    public class GetTradeDetailsQuery : IRequest<TradeDetailsDto>
    {
        public Guid TradeId { get; set; }
    }

    public class GetTradeDetailsQueryHandler : IRequestHandler<GetTradeDetailsQuery, TradeDetailsDto>
    {
        private readonly IJiggswapDb _db;

        public GetTradeDetailsQueryHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<TradeDetailsDto> Handle(GetTradeDetailsQuery request, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            return await conn.QuerySingleAsync<TradeDetailsDto>(@"
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
                    T.public_id = @TradeId", new
            {
                request.TradeId
            }).ConfigureAwait(false);
        }
    }
}