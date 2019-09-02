using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common.RabbitMqConfigs
{
    public static class RabbitConfigurationsLoader
    {
        public static RabbitConfig LoadConfigurations(string appsettingsFileName,bool isConsumer)
        {
            #region Init Config
            IConfiguration config = new ConfigurationBuilder()
                      .AddJsonFile("customer.consumer.appsettings.json", true, true)
                      .Build();
            var notificationsSection = config.GetSection("NotificationsAPI");
            string notificationsAPIURL = notificationsSection.GetSection("Address").Value;

            var rabbitSection = config.GetSection("RabbitMq");
            SystemConstants.ConnectionString = config.GetConnectionString("DbConnection");
            #endregion

            var rabbitConfig = new RabbitConfig
            {
                URL = rabbitSection.GetSection("Address").Value,
                Port = int.Parse(rabbitSection.GetSection("Port").Value),
                Username = rabbitSection.GetSection("UserName").Value,
                Password = rabbitSection.GetSection("Password").Value,
            };
            if(isConsumer)
            {
                rabbitConfig.ExchangeName = rabbitSection.GetSection("Exchang").Value;
                rabbitConfig.RoutingKey = rabbitSection.GetSection("Routing").Value;
                rabbitConfig.QueueName = rabbitSection.GetSection("QueueName").Value;
                rabbitConfig.ExchangeType = rabbitSection.GetSection("ExhangeType").Value;
                rabbitConfig.FailRetryCount = int.Parse(rabbitSection.GetSection("FailRetryCount").Value);
                rabbitConfig.FailRetryInterval = int.Parse(rabbitSection.GetSection("FailRetryInterval").Value);
                rabbitConfig.MaxConcurrentMessages = ushort.Parse(rabbitSection.GetSection("MaxConcurrentMessages").Value);
                rabbitConfig.DeadLetterExchange = rabbitSection.GetSection("DeadLetterExchange").Value;
                rabbitConfig.DeadLetterQueueName = rabbitSection.GetSection("DeadLetterQueueName").Value;
                rabbitConfig.Durable = rabbitSection.GetSection("Durable").Value == "yes" ? true : false;
                rabbitConfig.AutoDelete = rabbitSection.GetSection("AutoDelete").Value == "yes" ? true : false;
            }
            return rabbitConfig;

        }
    }
}
