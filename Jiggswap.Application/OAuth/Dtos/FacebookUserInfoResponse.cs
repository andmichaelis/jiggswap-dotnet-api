using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jiggswap.Application.OAuth.Dtos
{
    public class FacebookUserInfoResponse
    {
        [JsonPropertyName("id")]
        public string FacebookUserId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("picture")]
        public FacebookUserAvatar Picture { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class FacebookUserAvatar
    {
        [JsonPropertyName("data")]
        public FacebookUserAvatarData Data { get; set; }
    }

    public class FacebookUserAvatarData
    {
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}