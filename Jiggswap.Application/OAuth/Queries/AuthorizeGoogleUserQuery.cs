using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Jiggswap.Application.OAuth.Dtos;

namespace Jiggswap.Application.OAuth.Queries
{
    public class AuthorizeGoogleUserQuery : IRequest<OAuthUserData>
    {
        public string GoogleToken { get; set; }
    }

    public class AuthorizeGoogleUserQueryHandler : IRequestHandler<AuthorizeGoogleUserQuery, OAuthUserData>
    {
        private readonly string _clientId;

        public AuthorizeGoogleUserQueryHandler(IConfiguration config)
        {
            _clientId = config["OAuth:Google:CLIENT_ID"];
        }

        public async Task<OAuthUserData> Handle(AuthorizeGoogleUserQuery request, CancellationToken cancellationToken)
        {
            await Task.Run(() => true);

            Payload validatedUser;

            try
            {
                validatedUser = await ValidateAsync(request.GoogleToken, new ValidationSettings
                {
                    Audience = new[] { _clientId }
                });
            }
            catch
            {
                return new OAuthUserData { IsValid = false };
            }

            return new OAuthUserData
            {
                Email = validatedUser.Email,
                AvatarUrl = validatedUser.Picture,
                FirstName = validatedUser.GivenName,
                LastName = validatedUser.FamilyName,
                ServiceUserId = validatedUser.Subject,
                Service = OAuthService.Google,
                IsValid = true
            };
        }
    }
}