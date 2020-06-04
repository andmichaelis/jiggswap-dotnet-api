using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.OAuth.Dtos
{
    public class AuthorizeFacebookUserQueryResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string FacebookUserId { get; set; }

        public bool IsValid { get; set; }
    }
}