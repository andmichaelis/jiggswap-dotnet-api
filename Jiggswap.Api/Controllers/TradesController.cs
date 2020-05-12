using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Trades.Commands;
using Jiggswap.Application.Trades.Queries;
using Jiggswap.Application.Trades.Requests;
using Jiggswap.Application.Users.Queries;
using Jiggswap.Domain.Trades;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TradesController : BaseJiggswapController
    {
        private readonly ICurrentUserService _currentUserService;

        public TradesController(ICurrentUserService currentUserService, IMediator mediator) : base(mediator)
        {
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTrade([FromBody] CreateTradeCommand request)
        {
            var tradeId = await Mediator.Send(request).ConfigureAwait(false);

            var tradeDetails = await Mediator.Send(new GetTradeDetailsQuery
            {
                TradeId = tradeId
            }).ConfigureAwait(false);

            var emailRecipient = await Mediator.Send(new GetUserEmailFromUsernameQuery(tradeDetails.RequestedUsername))
                .ConfigureAwait(false);

            return Ok(tradeId);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetTrades()
        {
            var result = await Mediator.Send(new GetTradesForUserQuery
            {
                InternalUserId = _currentUserService.InternalUserId
            }).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost("accept")]
        public async Task<ActionResult<bool>> AcceptTrade(AcceptTradeRequest request)
        {
            await Mediator.Send(new UpdateTradeStateCommand(request.TradeId, TradeStates.Active)).ConfigureAwait(false);

            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<ActionResult> CancelTrade(CancelTradeRequest request)
        {
            await Mediator.Send(new UpdateTradeStateCommand(request.TradeId, TradeStates.Inactive)).ConfigureAwait(false);

            return Ok();
        }
    }
}