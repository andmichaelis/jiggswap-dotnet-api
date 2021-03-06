﻿using System;
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
        private readonly IJiggswapEmailer _emailer;

        public TradesController(ICurrentUserService currentUserService, IMediator mediator, IJiggswapEmailer emailer) : base(mediator)
        {
            _currentUserService = currentUserService;
            _emailer = emailer;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetTrades()
        {
            var result = await Mediator.Send(new GetTradesForUserQuery { InternalUserId = _currentUserService.InternalUserId });

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateTrade([FromBody] CreateTradeCommand request)
        {
            var tradeId = await Mediator.Send(request).ConfigureAwait(false);

            var tradeDetails = await Mediator.Send(new GetTradeDetailsQuery { TradeId = tradeId });

            var recipientEmail = await Mediator.Send(new GetUserEmailFromUsernameQuery(tradeDetails.RequestedUsername));

            _ = _emailer.SendNewTradeEmail(recipientEmail, tradeDetails);

            return Ok(tradeId);
        }

        [HttpPost("accept")]
        public async Task<ActionResult<bool>> AcceptTrade(AcceptTradeCommand request)
        {
            _ = await Mediator.Send(request);

            var tradeDetails = await Mediator.Send(new GetTradeDetailsQuery { TradeId = request.TradeId });

            var recipientEmail = await Mediator.Send(new GetUserEmailFromUsernameQuery(tradeDetails.InitiatorUsername));

            _ = _emailer.SendAcceptedTradeEmail(recipientEmail, tradeDetails);

            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<ActionResult> CancelTrade(CancelTradeCommand request)
        {
            _ = await Mediator.Send(request);

            return Ok();
        }

        [HttpPost("decline")]
        public async Task<ActionResult> DeclineTrade(DeclineTradeCommand request)
        {
            _ = await Mediator.Send(request);

            return Ok();
        }

        [HttpPost("shipped")]
        public async Task<ActionResult> ShippedTradePuzzle(ShippedTradeCommand command)
        {
            _ = await Mediator.Send(command);

            return Ok();
        }

        [HttpPost("received")]
        public async Task<ActionResult> ReceivedTradePuzzle(ReceivedTradeCommand command)
        {
            _ = await Mediator.Send(command);

            return Ok();
        }
    }
}