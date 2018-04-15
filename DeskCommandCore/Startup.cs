using System;
using AutoMapper.Configuration;
using DeskCommandCore;
using DeskCommandCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.Configure<Models.LayoutsConfig>(Configuration.GetSection("Layouts"));
            services.AddSignalR();
            services.AddMvc();

            services.AddTransient<ChatHub>();
            var configManager = new ConfigManagaer();

            var layoutsConfig = Configuration.GetSection("Layouts").Get<LayoutsConfig>();
            var layouts = configManager.ReadConfig(layoutsConfig);
            services.AddSingleton(layouts);
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

            app.UseMvc();

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chathub");
            });


            AutoMapper.Mapper.Initialize(new MapperConfigurationExpression { CreateMissingTypeMaps = true });
        }
    }
}
