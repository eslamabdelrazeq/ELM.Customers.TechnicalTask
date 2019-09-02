using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common.Bus.Services;
using ELM.Common.RabbitMqConfigs;
using ELM.Notifications.Handlers.Notifications;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ELM.Notifications.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region Masstransit
            services.AddScoped<NotificationsConsumeHandler>();
            services.AddMassTransit(c =>
            {
                c.AddConsumer<NotificationsConsumeHandler>();
            });
            var rabbitConfig = RabbitConfigurationsLoader.LoadConfigurations("appsettings.json");
            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(
              cfg =>
              {
                  var host = cfg.Host(rabbitConfig.URL, "/", settings => {
                      settings.Password(rabbitConfig.Password);
                      settings.Username(rabbitConfig.Username);
                  });
              }));
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IHostedService, BusService>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
