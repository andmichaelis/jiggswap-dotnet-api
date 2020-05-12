using Jiggswap.Api.Controllers;
using Jiggswap.Api.ResponseModels;
using Jiggswap.Api.Services;
using Jiggswap.Application.Users.Commands;
using Jiggswap.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Jiggswap.Application.Passwords.Commands;
using Jiggswap.Application.Passwords.Notifications;
using Jiggswap.Application.Users.Dtos;
using MediatR;

namespace JiggswapApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseJiggswapController
    {
        private readonly ITokenBuilder _tokenBuilder;

        public AuthController(ITokenBuilder tokenBuilder, IMediator mediator) : base(mediator)
        {
            _tokenBuilder = tokenBuilder;
        }

        [HttpPost("authorize")]
        [AllowAnonymous]
        public ActionResult<AuthorizedUserResponseWithToken> Authorize(UserSigninQuery request)
        {
            var result = new AuthorizedUserResponse
            {
                Username = request.Username
            };

            var token = _tokenBuilder.CreateToken(result);

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = request.Username,
                Token = token
            });
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> Signup(UserSignupCommand request)
        {
            var result = await Mediator.Send(request).ConfigureAwait(false);

            var token = _tokenBuilder.CreateToken(result);

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = result.Username,
                Token = token
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var resetToken = await Mediator.Send(command).ConfigureAwait(false);

            _ = Mediator.Publish(new ForgotPasswordNotification
            {
                Email = command.Email,
                Token = resetToken
            });

            return Ok();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var response = await Mediator.Send(command).ConfigureAwait(false);

            var token = _tokenBuilder.CreateToken(response);

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = response.Username,
                Token = token
            });
        }
    }
}