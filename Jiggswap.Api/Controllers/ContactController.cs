using Jiggswap.Application.Contact.Commands;
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
    public class ContactController : BaseJiggswapController
    {
        public ContactController(MediatR.IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<ActionResult> SubmitContact([FromBody] SubmitContactCommand command)
        {
            await Mediator.Send(command);

            return Ok();
        }
    }
}