using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jiggswap.Application.Images.Queries;
using Jiggswap.Application.Puzzles.Commands;
using Jiggswap.Application.Puzzles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ImageController : BaseJiggswapController
    {
        public ImageController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}"), ResponseCache(Duration = int.MaxValue)]
        public async Task<FileContentResult> Get([FromRoute] int id)
        {
            var data = await Mediator.Send(new GetImageQuery { ImageId = id }).ConfigureAwait(false);

            return new FileContentResult(data, "image/jpeg");
        }

        [HttpGet("{id}/b64")]
        public async Task<ActionResult<string>> GetBase64([FromRoute] int id)
        {
            var data = await Mediator.Send(new GetImageQuery { ImageId = id });

            return Ok(Convert.ToBase64String(data));
        }
    }
}