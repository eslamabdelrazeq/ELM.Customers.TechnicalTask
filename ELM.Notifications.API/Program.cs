using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ELM.Notifications.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
            var kestrelConfig = config.GetSection("Kestrel");
            var url = kestrelConfig.GetSection("Address").Value;
            var port = int.Parse(kestrelConfig.GetSection("Port").Value);

            var host = WebHost.CreateDefaultBuilder(args)
             .UseStartup<Startup>()
             //.UseUrls($"{url}:{port}")
             .UseKestrel((hostingContext, options) =>
             {
                 options.Listen(IPAddress.Any, port);
             })
            .Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
