using System;
using System.Threading.Tasks;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Emails;
using Jiggswap.Application.Trades.Commands;
using Jiggswap.Application.Trades.Queries;
using Jiggswap.Application.Users.Queries;
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
        private readonly IJiggswapNewTradeEmail _newTradeEmailer;

        public TradesController(ICurrentUserService currentUserService, IMediator mediator, IJiggswapNewTradeEmail newTradeEmail) : base(mediator)
        {
            _currentUserService = currentUserService;
            _newTradeEmailer = newTradeEmail;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTrade([FromBody] CreateTradeCommand request)
        {
            var tradeId = await Mediator.Send(request).ConfigureAwait(false);

            var tradeDetails = await Mediator.Send(new GetTradeDetailsQuery
            {
                TradeId = tradeId
            });

            var recipientEmail = await Mediator.Send(new GetUserEmailFromUsernameQuery(tradeDetails.RequestedUsername));

            await _newTradeEmailer.SendNewTradeEmail(recipientEmail, tradeDetails);

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
        public async Task<ActionResult<bool>> AcceptTrade(AcceptTradeCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<ActionResult> CancelTrade(CancelTradeCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }

        [HttpPost("decline")]
        public async Task<ActionResult> DeclineTrade(DeclineTradeCommand request)
        {
            await Mediator.Send(request);

            return Ok();
        }
    }
}