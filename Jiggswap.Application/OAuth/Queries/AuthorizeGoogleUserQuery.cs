using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Jiggswap.Application.OAuth.Queries
{
    public class AuthorizeGoogleUserQuery : IRequest<AuthorizeGoogleUserQueryResponse>
    {
        public string GoogleToken { get; set; }
    }

    public class AuthorizeGoogleUserQueryResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string GoogleUserId { get; set; }

        public bool IsValid { get; set; }
    }

    public class AuthorizeGoogleUserQueryHandler : IRequestHandler<AuthorizeGoogleUserQuery, AuthorizeGoogleUserQueryResponse>
    {
        private readonly string _clientId;

        private readonly string _clientSecret;

        public AuthorizeGoogleUserQueryHandler(IConfiguration config)
        {
            _clientId = config["OAuth:Google:CLIENT_ID"];
            _clientSecret = config["OAuth:Google:CLIENT_SECRET"];
        }

        public async Task<AuthorizeGoogleUserQueryResponse> Handle(AuthorizeGoogleUserQuery request, CancellationToken cancellationToken)
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
                return new AuthorizeGoogleUserQueryResponse { IsValid = false };
            }

            return new AuthorizeGoogleUserQueryResponse
            {
                Email = validatedUser.Email,
                AvatarUrl = validatedUser.Picture,
                FirstName = validatedUser.GivenName,
                LastName = validatedUser.FamilyName,
                GoogleUserId = validatedUser.Subject,
                IsValid = true
            };
        }
    }
}