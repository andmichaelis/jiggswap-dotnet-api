using FluentValidation.AspNetCore;
using Jiggswap.Api.Configuration;
using Jiggswap.Api.Services;
using Jiggswap.Application;
using Jiggswap.Application.Common.Interfaces;
using Jiggswap.RazorViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Utilities.Net;
using System.Net;

namespace JiggswapApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddTransient<ITokenBuilder, TokenBuilder>();
            services.AddTransient<IJiggswapCache, JiggswapCache>();

            services.AddJiggswapApplication(Configuration);
            services.AddJiggswapRazorViewEngine();

            services.AddHttpContextAccessor();

            services.AddJwt(Configuration["Jwt:Key"]);

            services.AddMemoryCache();

            services.AddCors();

            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ICurrentUserService>());

            services.AddRazorPages();

            services.Configure<ForwardedHeadersOptions>(opts =>
            {
                opts.KnownProxies.Add(System.Net.IPAddress.Parse("127.0.0.1"));
                opts.ForwardLimit = 2;
                opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseJiggswapRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}