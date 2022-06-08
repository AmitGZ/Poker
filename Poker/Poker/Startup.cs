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

            services.AddSignalR().AddAzureSignalR("Endpoint=https://pokersignalr.service.signalr.net;AccessKey=cxR4bYaXzsjLlNWRYxhUYpp4wGddY6kDzIM/GHqKAtE=;Version=1.0;");

            // services.AddCors(options =>
            // {
            //     options.AddDefaultPolicy(builder =>
            //     {
            //         builder.WithOrigins("https://pokerapplication.azurewebsites.net/") 
            //             .AllowAnyHeader()
            //             .AllowAnyMethod()
            //             .AllowCredentials();
            //     });
            // });

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder => {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("https://pokerapplication.azurewebsites.net/");
            }));

            services.AddSingleton<IDictionary<string, string>>(options => new Dictionary<string, string>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // app.UseCors();

            app.UseCors("CorsPolicy");

            app.UseAzureSignalR(endpoints =>
            {
                endpoints.MapHub<PokerHub>("/poker");
            });
        }
    }
}
