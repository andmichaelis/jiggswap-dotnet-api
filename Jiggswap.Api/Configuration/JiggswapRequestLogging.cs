using Dapper;
using Jiggswap.Application.Common;
using Jiggswap.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jiggswap.Api.Configuration
{
    public static class AddJiggswapRequestLogging
    {
        public static IApplicationBuilder UseJiggswapRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JiggswapRequestLogging>();
        }
    }

    public class JiggswapRequestLogging
    {
        private readonly RequestDelegate _next;

        public JiggswapRequestLogging(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJiggswapDb db, ICurrentUserService currentUser)
        {
            using (var conn = db.GetConnection())
            {
                var remoteIp = context.Request.Headers.ContainsKey("X-Forwarded-For") ?
                                context.Request.Headers["X-Forwarded-For"].ToString()
                                : context.Connection.RemoteIpAddress.ToString();

                await conn.ExecuteAsync(@"
                    insert into site_activity
                    (user_id, path, ip_address, http_method)
                    values
                    (@InternalUserId, @Path, @remoteIp, @Method)",
                    new
                    {
                        currentUser.InternalUserId,
                        Path = context.Request.Path.Value?.ToString() ?? string.Empty,
                        remoteIp,
                        context.Request.Method
                    });
            }

            await _next(context);
        }
    }
}