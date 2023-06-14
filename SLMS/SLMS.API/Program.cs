using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SLMS.Infrastructure.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
            services.AddHostedService<MessageConsumer>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
            webBuilder.UseSerilog((context, logger) =>
            {
             logger.ReadFrom.Configuration(context.Configuration);
            });
                webBuilder.UseStartup<Startup>();
            });

    }
}
