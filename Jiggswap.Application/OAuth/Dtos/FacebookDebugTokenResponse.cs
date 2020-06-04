using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jiggswap.Application.OAuth.Dtos
{
    public class FacebookDebugTokenResponse
    {
        [JsonPropertyName("data")]
        public FacebookDebugTokenData Data { get; set; }
    }

    public class FacebookDebugTokenData
    {
        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("application")]
        public string ApplicationName { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}