using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Notifications.Handlers.Notifications
{
    public class NotificationsConsumeHandler : IConsumer<RequestModel<List<NotificationDTO>>>
    {
        public NotificationsConsumeHandler()
        {

        }

        public async Task Consume(ConsumeContext<RequestModel<List<NotificationDTO>>> context)
        {
            Console.WriteLine($"Correlation message Id: {context.MessageId.Value}, Business message Id {context.Message.Header.MessageId}");
            foreach(var customer in context.Message.Body)
            {
                Console.WriteLine($"Mail has been sent to customer with email : {customer.Email}");
            }
        }
    }
}
