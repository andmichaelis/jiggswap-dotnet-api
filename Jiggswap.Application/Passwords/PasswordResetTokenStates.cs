using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Passwords
{
    public enum PasswordResetTokenStates
    {
        Active = 1,
        Used = 2,
        Invalidated = 3
    }
}
