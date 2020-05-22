using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Jiggswap.Application.Common;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Jiggswap.Application.Emails;

namespace Jiggswap.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJiggswapApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IJiggswapDb, JiggswapDb>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config.GetConnectionString("JiggswapRedis")));
            services.AddTransient<IJiggswapRedisConnection, JiggswapRedisConnection>();

            services.AddTransient<IJiggswapEmailer, JiggswapEmailer>();

            return services;
        }
    }
}