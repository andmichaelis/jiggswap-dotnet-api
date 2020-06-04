using Jiggswap.Application.OAuth.Dtos;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jiggswap.Application.OAuth.Queries
{
    public class AuthorizeFacebookUserQuery : IRequest<AuthorizeFacebookUserQueryResponse>
    {
        public string FacebookToken { get; set; }
    }

    public class AuthorizeFacebookUserQueryHandler : IRequestHandler<AuthorizeFacebookUserQuery, AuthorizeFacebookUserQueryResponse>
    {
        private readonly string _appId;

        private readonly string _appSecret;

        private readonly HttpClient _httpClient;

        public AuthorizeFacebookUserQueryHandler(IConfiguration config)
        {
            _appId = config["OAuth:Facebook:APP_ID"];
            _appSecret = config["OAuth:Facebook:APP_SECRET"];

            _httpClient = new HttpClient();
        }

        private async Task<string> GetFacebookAccessToken()
        {
            var response = await _httpClient.GetStringAsync("https://" +
                $"graph.facebook.com/oauth/access_token" +
                $"?client_id={_appId}" +
                $"&client_secret={_appSecret}" +
                $"&grant_type=client_credentials");

            return JsonSerializer.Deserialize<FacebookAccessTokenResponse>(response).AccessToken;
        }

        private async Task<FacebookDebugTokenData> GetFacebookDebugToken(string userToken, string accessToken)
        {
            var debugTokenUrl = "https://" + $"graph.facebook.com/debug_token?input_token={userToken}&access_token={accessToken}";

            var response = await _httpClient.GetStringAsync(debugTokenUrl);

            return JsonSerializer.Deserialize<FacebookDebugTokenResponse>(response).Data;
        }

        private async Task<FacebookUserInfoResponse> GetFacebookUserData(string userToken)
        {
            var infoUrl = "https://" + $"graph.facebook.com/me?fields=first_name,last_name,id,picture,email&access_token={userToken}";

            var response = await _httpClient.GetStringAsync(infoUrl);

            var deserialized = JsonSerializer.Deserialize<FacebookUserInfoResponse>(response);

            return deserialized;
        }

        private async Task<bool> ValidateJiggswapFacebookToken(string userToken)
        {
            var accessToken = await GetFacebookAccessToken();

            var debugToken = await GetFacebookDebugToken(userToken, accessToken);

            return debugToken.IsValid;
        }

        public async Task<AuthorizeFacebookUserQueryResponse> Handle(AuthorizeFacebookUserQuery request, CancellationToken cancellationToken)
        {
            await Task.Delay(1);

            var isValidJiggswapToken = await ValidateJiggswapFacebookToken(request.FacebookToken);

            if (!isValidJiggswapToken)
            {
                return new AuthorizeFacebookUserQueryResponse { IsValid = false };
            }

            var userData = await GetFacebookUserData(request.FacebookToken);

            return new AuthorizeFacebookUserQueryResponse
            {
                IsValid = true,
                AvatarUrl = userData.Picture.Data.IsSilhouette ? string.Empty : userData.Picture.Data.Url,
                Email = userData.Email,
                FacebookUserId = userData.FacebookUserId,
                FirstName = userData.FirstName,
                LastName = userData.LastName
            };
        }
    }
}