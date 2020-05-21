using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJiggswapRazorViewEngine(this IServiceCollection services)
        {
            services.AddScoped<IJiggswapRazorViewRenderer, JiggswapRazorViewRenderer>();

            return services;
        }
    }
}