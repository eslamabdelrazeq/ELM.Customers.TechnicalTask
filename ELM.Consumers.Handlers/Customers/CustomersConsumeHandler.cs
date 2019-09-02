using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Interfaces.CustomerService;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Consumers.Handlers.Customers
{
    public class CustomersConsumeHandler : IConsumer<RequestModel<List<CustomerDTO>>>
    {
        public static ServiceProvider ServiceProvider;
        public static HttpClient HttpClient;
        public static string NotificationsAPIURL;
        public CustomersConsumeHandler()
        {

        }

        public async Task Consume(ConsumeContext<RequestModel<List<CustomerDTO>>> context)
        {
            Console.WriteLine($"Correlation message Id: {context.MessageId.Value}, Business message Id {context.Message.Header.MessageId}");
            var customersService = ServiceProvider.GetRequiredService<ICustomerService>();
            #region DB Insertion
            await customersService.CreateCustomers(context.Message);
            #endregion

            #region Notify Notifications API

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{NotificationsAPIURL}");
            request.Content = new StringContent(JsonConvert.SerializeObject(context.Message), Encoding.UTF8, "application/json");
            try
            {
                var response = await HttpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Notifications API is not reachable",ex.Message);
            }
            #endregion
        }
    }
}
