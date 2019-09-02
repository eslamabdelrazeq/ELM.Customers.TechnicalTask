using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Consumer.Handlers
{
    public class CustomersConsumeHandler : IConsumer<RequestModel<List<CustomerDTO>>>
    {
        public Task Consume(ConsumeContext<RequestModel<List<CustomerDTO>>> context)
        {
            Console.WriteLine($"Receive message value: {context.MessageId.Value}");
            return Task.CompletedTask;
        }
    }
}
