using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        int InternalUserId { get; }

        string Username { get; }
    }
}