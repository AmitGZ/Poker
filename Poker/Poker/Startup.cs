using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Poker.Hubs;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using Poker;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using PokerClassLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Owin;

namespace Poker
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<PokerContext>(options =>
               options.UseLazyLoadingProxies()
               .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),
               ServiceLifetime.Singleton);

            services.AddSignalR().AddAzureSignalR("Endpoint=https://pokersignalr.service.signalr.net;AccessKey=c6LDgHAz7IJK6BkZn2SX0GcsUmXLq4RIQ3BND23BSHc=;Version=1.0;");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddSingleton<IDictionary<string, string>>(options => new Dictionary<string, string>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseRouting();

            app.UseCors();

            app.UseAzureSignalR(endpoints =>
            {
                endpoints.MapHub<PokerHub>("/poker");
            });
        }
    }
}
