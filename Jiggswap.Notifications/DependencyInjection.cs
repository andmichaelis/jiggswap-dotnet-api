using System.Reflection;
using MediatR;
using Jiggswap.Notifications.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Jiggswap.Notifications
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJiggswapNotifications(this IServiceCollection services)
        {
            services.AddTransient<IJiggswapNotifier, JiggswapNotifier>();

            return services;
        }
    }
}