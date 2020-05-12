using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jiggswap.Application.Puzzles.Commands;
using Jiggswap.Application.Puzzles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PuzzlesController : BaseJiggswapController
    {
        public PuzzlesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PuzzleListItem>>> GetPuzzles([FromQuery] GetPuzzlesListQuery request)
        {
            var result = await Mediator.Send(request).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetPuzzleDetails([FromRoute] Guid id)
        {
            var result = await Mediator.Send(new GetPuzzleDetailsQuery { PuzzleId = id }).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<ActionResult> CreatePuzzle([FromForm] CreatePuzzleCommand command)
        {
            var instance = command as CreatePuzzleCommand;

            var result = await Mediator.Send(instance).ConfigureAwait(false);

            await Mediator.Send(new CreatePuzzleImageCommand
            {
                PuzzleId = result,
                ImageBlob = command.ImageBlob
            }).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> UpdatePuzzle([FromRoute] Guid id, [FromForm] UpdatePuzzleCommand command)
        {
            command.PuzzleId = id;

            var result = await Mediator.Send(command).ConfigureAwait(false);

            await Mediator.Send(new CreatePuzzleImageCommand
            {
                PuzzleId = result,
                ImageBlob = command.ImageBlob
            }).ConfigureAwait(false);

            return Ok(result);
        }
    }
}