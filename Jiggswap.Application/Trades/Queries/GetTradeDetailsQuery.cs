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

            return await conn.QuerySingleAsync<TradeDetailsDto>(@$"
                {GetTradesForUserQueryHandler.SqlQuery}
                where
                    T.public_id = @TradeId",
                    new
                    {
                        TradeStates.Active,
                        request.TradeId
                    });
        }
    }
}