using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Emails;
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
        private readonly IJiggswapEmailer _emailer;
        private readonly ICurrentUserService _currentUser;

        public PuzzlesController(IMediator mediator, IJiggswapEmailer emailer, ICurrentUserService currentUser) : base(mediator)
        {
            _emailer = emailer;
            _currentUser = currentUser;
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
        public async Task<ActionResult<Guid>> CreatePuzzle([FromForm] CreatePuzzleCommand command)
        {
            var result = await Mediator.Send(command).ConfigureAwait(false);

            _ = _emailer.SendAdminEmail("New Puzzle Created", $"User: {_currentUser.Username}. Puzzle: {command.Title}!");

            _ = await Mediator.Send(new CreatePuzzleImageCommand
            {
                PuzzleId = result,
                ImageBlob = command.ImageBlob
            }).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<Guid>> UpdatePuzzle([FromRoute] Guid id, [FromForm] UpdatePuzzleCommand command)
        {
            command.PuzzleId = id;

            var result = await Mediator.Send(command).ConfigureAwait(false);

            _ = await Mediator.Send(new CreatePuzzleImageCommand
            {
                PuzzleId = result,
                ImageBlob = command.ImageBlob
            }).ConfigureAwait(false);

            return Ok(id);
        }
    }
}