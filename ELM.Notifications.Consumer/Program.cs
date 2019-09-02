using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ELM.Notifications.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("I'm the notifications consumer POC");
            #region Init Config
            IConfiguration config = new ConfigurationBuilder()
                      .AddJsonFile("notifications.consumer.appsettings.json", true, true)
                      .Build();
            var notificationsSection = config.GetSection("NotificationsAPI");
            string notificationsAPIURL = notificationsSection.GetSection("Address").Value;

            var rabbitSection = config.GetSection("RabbitMq");
            string rabbitURL = rabbitSection.GetSection("Address").Value;
            int rabbitPort = int.Parse(rabbitSection.GetSection("Port").Value);

            string exchange = rabbitSection.GetSection("Exchang").Value;
            string routingKey = rabbitSection.GetSection("Routing").Value;
            string exchangeType = rabbitSection.GetSection("ExhangeType").Value;

            #endregion


            #region Queue Settings
            var factory = new ConnectionFactory() { HostName = rabbitURL };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchange,
                                        type: exchangeType);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: exchange,
                                  routingKey: routingKey);


                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var mroutingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}' Mail has been sent to customer",
                                      mroutingKey, message);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            #endregion

        }
    }
}