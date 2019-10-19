using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DnsClient;
using SD.Api.Dtos;
using System.Net.Http;

namespace SD.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));
            services.AddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                var dnsEndpoint = serviceConfiguration.Consul.DnsEndpoint;

                //推荐
                return new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
                //或者
                //return new LookupClient(IPAddress.Parse(dnsEndpoint.Address), dnsEndpoint.Port);
                //return new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

            });

            //services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            //{
            //    var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

            //    if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
            //    {
            //        // if not configured, the client will use the default value "127.0.0.1:8500"
            //        cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
            //    }
            //}));

            services.AddSingleton<HttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
