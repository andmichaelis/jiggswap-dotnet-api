using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Application.Common
{
    public interface IJiggswapRedisConnection
    {
        public IConnectionMultiplexer Redis { get; }
    }

    public class JiggswapRedisConnection : IJiggswapRedisConnection
    {
        public IConnectionMultiplexer Redis { get; }

        public JiggswapRedisConnection(IConnectionMultiplexer redis)
        {
            Redis = redis;
        }
    }
}