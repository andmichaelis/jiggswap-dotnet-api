using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Passwords.Dtos
{
    public class PasswordResetTokenDto
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public PasswordResetTokenStates Status { get; set; }
    }
}