using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ELM.Customers.Producer
{
    public class NotificationExchangePublisher
    {

        public static void PublishMessage(string message)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("notification.producer.appsettings.json", true, true)
              .Build();

            var rabbitSection = config.GetSection("RabbitMq");
            string rabbitURL = rabbitSection.GetSection("Address").Value;
            int rabbitPort = int.Parse(rabbitSection.GetSection("Port").Value);

            string exchange = rabbitSection.GetSection("Exchang").Value;
            string routingKey = rabbitSection.GetSection("Routing").Value;
            string exchangeType = rabbitSection.GetSection("ExhangeType").Value;

            var factory = new ConnectionFactory() { HostName = rabbitURL };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: exchange,
                                        type: exchangeType);

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Queue Message Sent '{0}'", message);
        }
    }
}
