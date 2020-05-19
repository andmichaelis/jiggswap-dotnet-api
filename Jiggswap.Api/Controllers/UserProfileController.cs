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
    public class UserProfileController : BaseJiggswapController
    {
        private readonly ICurrentUserService _currentUserService;

        public UserProfileController(ICurrentUserService currentUserService, IMediator mediator) : base(mediator)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<PublicProfileDto>> GetPublicProfile([FromRoute] string username)
        {
            var result = await Mediator.Send(new GetPublicProfileQuery(username)).ConfigureAwait(false);

            return result;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<ActionResult<PrivateProfileDto>> GetPrivateProfile()
        {
            return await Mediator.Send(
                new GetPrivateProfileQuery
                {
                    UserId = _currentUserService.InternalUserId
                });
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