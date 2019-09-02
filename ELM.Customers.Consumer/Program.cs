using ELM.Common;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Entities;
using ELM.Common.Interfaces.CustomerService;
using ELM.Common.RabbitMqConfigs;
using ELM.Consumers.Handlers.Customers;
using ELM.Customers.Database.Context;
using ELM.Customers.Database.DAL;
using ELM.Customers.Services.Customer;
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace ELM.Customers.Consumer
{
    class Program
    {
        private static string appsettingsFileName = "customer.consumer.appsettings.json";
        static void Main(string[] args)
        {
            Console.WriteLine("Hi, I'm the customer's consumer POC");
            #region DI
            IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile($"{appsettingsFileName}", true, true)
          .Build();
            var notificationsSection = config.GetSection("NotificationsAPI");
            string notificationsAPIURL = notificationsSection.GetSection("Address").Value;
            var serviceProvider = new ServiceCollection()
           .AddLogging()
           .AddScoped<ICustomerService, CustomerService>()
           .AddScoped(typeof(DbContext), typeof(ELMCustomersDbContext))
           .AddHttpClient()
           .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
           .BuildServiceProvider();

            //Should add polly and confiugre retry policy here
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient();
            CustomersConsumeHandler.ServiceProvider = serviceProvider;
            CustomersConsumeHandler.HttpClient = client;
            CustomersConsumeHandler.NotificationsAPIURL = notificationsAPIURL;
            #endregion
            #region MassTransit
            RunMassTransitReceiverWithRabbitMq();
            #endregion
        }

        private static void RunMassTransitReceiverWithRabbitMq()
        {
            var rabbitConfig = RabbitConfigurationsLoader.LoadConfigurations(appsettingsFileName, true);
            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                var host = rabbit.Host(rabbitConfig.URL, "/", settings =>
                {
                    settings.Password(rabbitConfig.Password);
                    settings.Username(rabbitConfig.Username);
                });

                rabbit.ReceiveEndpoint(host, rabbitConfig.QueueName, conf =>
                {
                    conf.PrefetchCount = rabbitConfig.MaxConcurrentMessages;
                    conf.ExchangeType = rabbitConfig.ExchangeType;
                    conf.Durable = rabbitConfig.Durable;
                    conf.AutoDelete = rabbitConfig.AutoDelete;
                    conf.DeadLetterExchange = rabbitConfig.DeadLetterExchange;
                    conf.BindDeadLetterQueue(rabbitConfig.DeadLetterExchange, rabbitConfig.DeadLetterQueueName);
                    conf.UseMessageRetry(r => r.Interval(rabbitConfig.FailRetryCount, rabbitConfig.FailRetryInterval));
                    conf.Consumer<CustomersConsumeHandler>();
                });
            });
            rabbitBusControl.Start();
            Console.ReadKey();
            rabbitBusControl.Stop();
        }
    }
}
