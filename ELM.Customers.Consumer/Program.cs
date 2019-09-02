using ELM.Common;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Entities;
using ELM.Customers.Database.Context;
using ELM.Customers.Database.DAL;
using ELM.Customers.Services.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace ELM.Customers.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("I'm the customers consumer POC");
            #region Init Config
            IConfiguration config = new ConfigurationBuilder()
                      .AddJsonFile("customer.consumer.appsettings.json", true, true)
                      .Build();
            var notificationsSection = config.GetSection("NotificationsAPI");
            string notificationsAPIURL = notificationsSection.GetSection("Address").Value;

            var rabbitSection = config.GetSection("RabbitMq");
            string rabbitURL = rabbitSection.GetSection("Address").Value;
            int rabbitPort = int.Parse(rabbitSection.GetSection("Port").Value);

            string exchange = rabbitSection.GetSection("Exchang").Value;
            string routingKey = rabbitSection.GetSection("Routing").Value;
            string exchangeType = rabbitSection.GetSection("ExhangeType").Value;

            SystemConstants.ConnectionString = config.GetConnectionString("DbConnection");
            var serviceProvider = new ServiceCollection()
           .AddLogging()
           .AddScoped<ICustomerService, CustomerService>()
           .AddScoped(typeof(DbContext), typeof(ELMCustomersDbContext))
           .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
           .BuildServiceProvider();
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
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      mroutingKey, message);

                    var customrsRespobse = JsonConvert.DeserializeObject<HttpRequestModel<List<CustomerDTO>>>(message);
                    var customerService = serviceProvider.GetRequiredService<ICustomerService>();
                    await customerService.CreateCustomers(customrsRespobse);

                    var serviceProviderr = new ServiceCollection()
                    .AddHttpClient()
                    .BuildServiceProvider();

                    //Should add polly and confiugre retry policy here
                    var httpClientFactory = serviceProviderr.GetService<IHttpClientFactory>();
                    var client = httpClientFactory.CreateClient();


                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{notificationsAPIURL}");
                    request.Content = new StringContent(message, Encoding.UTF8, "application/json");
                    try
                    {
                        var response = await client.SendAsync(request);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


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
