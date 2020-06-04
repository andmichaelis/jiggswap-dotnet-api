using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jiggswap.Application.OAuth.Dtos
{
    public class FacebookAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}