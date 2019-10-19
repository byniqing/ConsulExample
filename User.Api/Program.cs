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

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile(
                    path: "appsettings.json",
                    optional: true,
                    reloadOnChange: false);
            })
                //.UseKestrel(o =>
                //    {
                //        o.Listen(IPAddress.IPv6Loopback, 5080); //HTTP port
                //        //o.Listen(IPAddress.Loopback, 5443); //HTTPS port
                //    })
               //.UseUrls("http://39.105.144.51:90")
               //.UseUrls("http://*:6001")
                //.UseUrls("http://*:80")
                .UseStartup<Startup>();
    }
}
