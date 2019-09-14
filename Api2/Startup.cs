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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
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

            //InitTable(app);

            lifetime.ApplicationStarted.Register(OnStart);
            //lifetime.ApplicationStopped.Register(OnStoped);
        }

        private void OnStart()
        {
            //uses default host:port which is localhost:8500
            var client = new ConsulClient();

            //健康检查
            var httpCheck = new AgentServiceCheck()
            {
                //一分钟不健康，则下线你的服务，从consul中移除掉
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(10), //健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                HTTP = "http://localhost:5002/api/HealthCheck" //健康检查地址
            };

            //注册
            var agentReg = new AgentServiceRegistration()
            {
                ID = "service2",
                Check = httpCheck,
                Address = "localhost",
                Name = "UserService",
                Port = 5002,
            };

            //ConfigureAwait(false); 指同步
            client.Agent.ServiceRegister(agentReg).ConfigureAwait(false);
        }
        private void OnStoped()
        {
            //uses default host:port which is localhost:8500
            var client = new ConsulClient();
            client.Agent.ServiceDeregister("servicename:5005");
        }
    }
}
