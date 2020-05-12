using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jiggswap.Api.Services;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Profiles.Commands;
using Jiggswap.Application.Profiles.Dtos;
using Jiggswap.Application.Profiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jiggswap.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfilesController : BaseJiggswapController
    {
        private readonly ICurrentUserService _currentUserService;

        public UserProfilesController(ICurrentUserService currentUserService, IMediator mediator) : base(mediator)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<PublicProfileResult>> GetPublicProfile([FromRoute] string username)
        {
            var result = await Mediator.Send(new GetPublicProfileQuery(username)).ConfigureAwait(false);

            return result;
        }

        [HttpGet("{username}/private")]
        [Authorize]
        public async Task<ActionResult<PrivateProfileDto>> GetPrivateProfile([FromRoute] string username)
        {
            if (username != _currentUserService.Username)
            {
                return BadRequest("Can't view private profiles of other users.");
            }

            return await Mediator.Send(new GetPrivateProfileQuery(username)).ConfigureAwait(false);
        }

        [HttpPut("{username}")]
        [Authorize]
        public async Task<ActionResult<bool>> SaveProfile([FromRoute] string username, [FromBody] SaveProfileCommand command)
        {
            if (username != _currentUserService.Username)
            {
                return BadRequest("Can't update other users.");
            }

            var result = await Mediator.Send(command).ConfigureAwait(false);

            return Ok(result);
        }
    }
}