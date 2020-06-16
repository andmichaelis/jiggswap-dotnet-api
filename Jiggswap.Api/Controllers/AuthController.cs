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
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Jiggswap.Application.OAuth.Queries;
using Jiggswap.Application.OAuth.Commands;
using Jiggswap.Application.OAuth.Dtos;
using System;

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

        [HttpPost("authorize/jiggswap")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> AuthorizeJiggswap(UserSigninQuery request)
        {
            var isEmail = request.UsernameOrEmail.Contains('@');

            string username = isEmail
                ? await Mediator.Send(new GetUsernameFromEmailQuery(request.UsernameOrEmail))
                : await Mediator.Send(new GetUsernameFromUsernameQuery(request.UsernameOrEmail));

            var result = new AuthorizedUserResponse { Username = username };

            var token = _tokenBuilder.CreateToken(result);

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = username,
                Token = token
            });
        }

        [HttpPost("authorize/google")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> AuthorizeGoogle(AuthorizeGoogleUserQuery request)
        {
            var oauthData = await Mediator.Send(request);

            if (!oauthData.IsValid)
            {
                return BadRequest();
            }

            return await AuthorizeOauthRequest(oauthData, request.Username);
        }

        [HttpPost("authorize/facebook")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> AuthorizeFacebook(AuthorizeFacebookUserQuery request)
        {
            var oauthData = await Mediator.Send(request);

            if (!oauthData.IsValid)
            {
                return BadRequest();
            }

            return await AuthorizeOauthRequest(oauthData, request.Username);
        }

        private async Task<ActionResult> AuthorizeOauthRequest(OAuthUserData oauthData, string chosenUsername)
        {
            var linkedAccount = await Mediator.Send(new OAuthFindLinkedAccountQuery(oauthData.Service, oauthData.ServiceUserId));

            if (!string.IsNullOrEmpty(linkedAccount.Username))
            {
                return Ok(new AuthorizedUserResponseWithToken
                {
                    Username = linkedAccount.Username,
                    Token = _tokenBuilder.CreateToken(linkedAccount)
                });
            }

            if (string.IsNullOrEmpty(oauthData.Email))
            {
                return Ok(new { NeedEmail = true });
            }

            return Ok(new { NeedUsername = true });
        }

        [HttpPost("signup/jiggswap")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> Signup(UserSignupCommand request)
        {
            var result = await Mediator.Send(request).ConfigureAwait(false);

            var token = _tokenBuilder.CreateToken(result);

            _ = _emailer.SendAdminEmail("New User Signed Up", $"User: {request.Username} <{request.Email}>!");

            return Ok(new AuthorizedUserResponseWithToken
            {
                Username = result.Username,
                Token = token
            });
        }

        [HttpPost("signup/google")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> SignupGoogle(OAuthSignupQuery request)
        {
            // Validated Username

            var oauthData = await Mediator.Send(new AuthorizeGoogleUserQuery { Token = request.Token, Username = request.Username });

            if (!oauthData.IsValid)
            {
                return BadRequest();
            }

            return await SignupOAuthRequest(oauthData, request.Username);
        }

        [HttpPost("signup/facebook")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorizedUserResponseWithToken>> SignupFacebook(OAuthSignupQuery request)
        {
            // Validated Username

            var oauthData = await Mediator.Send(new AuthorizeFacebookUserQuery { Token = request.Token, Username = request.Username });

            if (!oauthData.IsValid)
            {
                return BadRequest();
            }

            return await SignupOAuthRequest(oauthData, request.Username);
        }

        private async Task<ActionResult<AuthorizedUserResponseWithToken>> SignupOAuthRequest(OAuthUserData oauthData, string username)
        {
            var user = await Mediator.Send(new OAuthUserSigninCommand { OAuthData = oauthData, Username = username });

            return Ok(new AuthorizedUserResponseWithToken
            {
                Token = _tokenBuilder.CreateToken(user),
                Username = user.Username
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