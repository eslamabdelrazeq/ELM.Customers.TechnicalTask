using System;
using System.Collections.Generic;
using System.Text;

namespace ELM.Common
{
    public class RabbitConfig
    {
        public string URL { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string ExchangeType { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public string RoutingKey { get; set; }
        public ushort MaxConcurrentMessages { get; set; }
        public string DeadLetterExchange { get; set; }
        public string DeadLetterQueueName { get; set; }
        public int FailRetryCount { get; set; }
        public int FailRetryInterval { get; set; }

    }
}
