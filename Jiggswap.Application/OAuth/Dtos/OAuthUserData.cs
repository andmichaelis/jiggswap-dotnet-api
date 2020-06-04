using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.OAuth.Dtos
{
    public class OAuthUserData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string ServiceUserId { get; set; }

        public OAuthService Service { get; set; }

        public bool IsValid { get; set; }
    }

    public enum OAuthService
    {
        Google,
        Facebook
    }
}