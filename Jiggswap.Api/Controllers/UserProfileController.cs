using System.Threading.Tasks;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.Application.Images.Commands;
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
            return await Mediator.Send(new GetPublicProfileQuery(username)).ConfigureAwait(false);
        }

        [HttpGet("")]
        [Authorize]
        public async Task<ActionResult<PrivateProfileDto>> GetPrivateProfile()
        {
            return await Mediator.Send(
                new GetPrivateProfileQuery { UserId = _currentUserService.InternalUserId });
        }

        [HttpPut()]
        [Authorize]
        public async Task<ActionResult> SaveProfile([FromForm] SaveProfileCommand command)
        {
            var profileId = await Mediator.Send(command).ConfigureAwait(false);

            if (command.ImageBlob != null)
            {
                var imageId = await Mediator.Send(new SaveImageCommand(command.ImageBlob));

                _ = await Mediator.Send(new SaveAvatarCommand
                {
                    ProfileId = profileId,
                    ImageId = imageId
                }).ConfigureAwait(false);
            }

            return Ok();
        }
    }
}