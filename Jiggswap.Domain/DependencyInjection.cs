using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJiggswapEntityFramework(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<JiggswapEfContext>(opts =>
            {
                opts.UseNpgsql(config.GetConnectionString("JiggswapDb"));
            });

            return services;
        }
    }
}