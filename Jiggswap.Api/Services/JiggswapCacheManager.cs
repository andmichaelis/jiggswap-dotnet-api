using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Jiggswap.Application.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Jiggswap.Api.Services
{
    public interface IJiggswapCache
    {
        int GetInternalUserId(string username);
    }

    public class JiggswapCache : IJiggswapCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IJiggswapDb _db;

        public JiggswapCache(IMemoryCache memoryCache, IJiggswapDb db)
        {
            _memoryCache = memoryCache;
            _db = db;
        }

        public int GetInternalUserId(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return -1;
            }

            var key = $"{username}_id";

            if (!_memoryCache.TryGetValue(key, out int userId))
            {
                using (var conn = _db.GetConnection())
                {
                    userId = conn.QuerySingle<int>("select id from users where username = @Username", new { Username = username });
                }

                _memoryCache.Set<int>(key, userId, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                });
            }

            return userId;
        }
    }
}
