using ELM.Common.RabbitMqConfigs;
using ELM.Notifications.Handlers.Notifications;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ELM.Notifications.Consumer
{
    class Program
    {
        private static string appsettingsFileName = "notifications.consumer.appsettings.json";
        static void Main(string[] args)
        {
            Console.WriteLine("I'm the notification's consumer POC");
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
                    conf.Consumer<NotificationsConsumeHandler>();
                });
            });
            rabbitBusControl.Start();
            Console.ReadKey();
            rabbitBusControl.Stop();
        }

    }
}