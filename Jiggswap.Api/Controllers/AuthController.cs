using Jiggswap.Api.Controllers;
using Jiggswap.Api.ResponseModels;
using Jiggswap.Api.Services;
using Jiggswap.Application.Users.Commands;
using Jiggswap.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Jiggswap.Application.Passwords.Commands;
using Jiggswap.Application.Users.Dtos;
using MediatR;
using Jiggswap.Application.Emails;

namespace JiggswapApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseJiggswapController
    {
        private readonly ITokenBuilder _tokenBuilder;
        private readonly IJiggswapEmailer _emailer;

        public AuthController(ITokenBuilder tokenBuilder, IMediator mediator, IJiggswapEmailer emailer) : base(mediator)
        {
            _tokenBuilder = tokenBuilder;
            _emailer = emailer;
        }

        [HttpPost("authorize")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> Authorize(UserSigninQuery request)
        {
            var isEmail = request.UsernameOrEmail.Contains('@');

            var username = request.UsernameOrEmail;

            if (isEmail)
            {
                username = await Mediator.Send(new GetUsernameFromEmailQuery(request.UsernameOrEmail));
            }

            var result = new AuthorizedUserResponse { Username = username };

            var token = _tokenBuilder.CreateToken(result);

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = username,
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

            _ = _emailer.SendForgotPasswordEmail(command.Email, resetToken);

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