using System;
using System.Security.Claims;
using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Jiggswap.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IJiggswapCache cache)
        {
            Username = httpContextAccessor.HttpContext?.User?.Identity?.Name;

            InternalUserId = cache.GetInternalUserId(Username);
        }

        public string Username { get; }

        public int InternalUserId { get; }
    }
}