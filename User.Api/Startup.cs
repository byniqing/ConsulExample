using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api
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
            //注入
            //services.AddDbContext<DbUserContext>(options =>
            //{
            //    //options.UseMySQL(Configuration.GetConnectionString("Sql"));
            //    options.UseSqlServer(Configuration.GetConnectionString("Sql"));
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddOptions();
            services.Configure<ServiceDiscoveryOptions>(Configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<IConsulClient>(p => new ConsulClient(cfg =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;

                if (!string.IsNullOrEmpty(serviceConfiguration.Consul.HttpEndpoint))
                {
                    // if not configured, the client will use the default value "127.0.0.1:8500"
                    cfg.Address = new Uri(serviceConfiguration.Consul.HttpEndpoint);
                }
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime lifetime,
        ILoggerFactory loggerFactory,
        IOptions<ServiceDiscoveryOptions> serviceOptions,
        IConsulClient consul)
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

            //app.UseHttpsRedirection();
            app.UseMvc();

            //InitTable(app);

            lifetime.ApplicationStarted.Register(() =>
            {
                RegisterService(app, lifetime, serviceOptions, consul);
            });
            //lifetime.ApplicationStopped.Register(OnStoped);
        }

        private void RegisterService(IApplicationBuilder app, IApplicationLifetime appLife,
     IOptions<ServiceDiscoveryOptions> serviceOptions,
     IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                //健康检查
                var httpCheck = new AgentServiceCheck()
                {
                    /*
                     服务不通了。多久后移除
                     */
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "api/HealthCheck").OriginalString
                };

                //服务注册
                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    /*
                     测试的时候用 的localhost
                     那么在DnsClient取值的时候
                     AddressList会为空，不过HostName有值
                     */
                    Address = address.Host, // "127.0.0.1",
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                /*
                 服务停止，主动从consul集群异常服务
                 当然，如果不主动移除，consul集群也会移除，配置健康检查即可：DeregisterCriticalServiceAfter
                 */
                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }
        }

        private void DeRegisterService(IApplicationBuilder app,
   IOptions<ServiceDiscoveryOptions> serviceOptions,
   IConsulClient consul)
        {
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
            }
        }

        //public void InitTable(IApplicationBuilder app)
        //{
        //    using (var scope = app.ApplicationServices.CreateScope())
        //    {
        //        var user = scope.ServiceProvider.GetService<DbUserContext>();
        //        user.Database.Migrate(); //相当于手动执行 update-database，但必须要又migrations
        //        //if (!User.User.Any())
        //        if (user.User.Count() <= 0)
        //        {
        //            user.User.Add(new Model.AppUser { Name = "cnblogs", Company = "博客园" });
        //            user.SaveChanges();
        //        }
        //    }
        //}
    }
}
