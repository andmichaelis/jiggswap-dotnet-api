using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using MediatR;

namespace Jiggswap.Application.Trades.Commands
{
    public class UpdateTradeStateCommand : IRequest<bool>
    {
        public Guid TradeId { get; set; }

        public string Status { get; set; }

        public UpdateTradeStateCommand(Guid tradeId, string status)
        {
            TradeId = tradeId;
            Status = status;
        }
    }

    public class UpdateTradeStateCommandHandler : IRequestHandler<UpdateTradeStateCommand, bool>
    {
        private readonly IJiggswapDb _db;

        public UpdateTradeStateCommandHandler(IJiggswapDb db)
        {
            _db = db;
        }

        public async Task<bool> Handle(UpdateTradeStateCommand command, CancellationToken cancellationToken)
        {
            using var conn = _db.GetConnection();

            await conn.ExecuteAsync("update trades set status = @Status where Public_Id = @TradeId",
                new
                {
                    command.Status,
                    command.TradeId
                }).ConfigureAwait(false);

            return true;
        }
    }
}
