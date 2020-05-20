using Jiggswap.Application.Feedback.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class FeedbackController : BaseJiggswapController
    {
        public FeedbackController(MediatR.IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<ActionResult> SubmitFeedback([FromBody] SubmitFeedbackCommand command)
        {
            await Mediator.Send(command);

            return Ok();
        }
    }
}