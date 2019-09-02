using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Interfaces.CustomerService;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Common.Consumers.Customers
{
    public class CustomersConsumeHandler : IConsumer<RequestModel<List<CustomerDTO>>>
    {
        public CustomersConsumeHandler()
        {

        }

        public Task Consume(ConsumeContext<RequestModel<List<CustomerDTO>>> context)
        {
            Console.WriteLine($"Receive message value: {context.MessageId.Value}");
            var serviceProvider = new ServiceCollection();
            return Task.CompletedTask;
        }
    }
}

