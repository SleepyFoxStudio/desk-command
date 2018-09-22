using System;
using AutoMapper.Configuration;
using DeskCommandCore;
using DeskCommandCore.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace DeskCommandCore
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.Configure<Models.Config.Layouts>(Configuration.GetSection("Layouts"));
            services.AddSignalR();
            services.AddMvc();
            services.AddMediatR();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod().AllowAnyHeader()
                        .WithOrigins("http://localhost:59584")
                        .AllowCredentials();
                }));

            services.AddSingleton<ConfigurationMonitor>();
            services.AddSingleton<LayoutProvider>();
            services.AddSingleton<ClientUiManager>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    
                }
            });

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chathub");
            });

            app.UseMvc();

            
            AutoMapper.Mapper.Initialize(new MapperConfigurationExpression { CreateMissingTypeMaps = true });
        }
    }
}
