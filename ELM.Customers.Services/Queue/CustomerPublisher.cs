using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Customers.Producer;
using Newtonsoft.Json;

namespace ELM.Customers.Services.Queue
{
    public class CustomerPublisher : ICustomerPublisher
    {
        public CustomerPublisher()
        {
                
        }
        public async Task<ResponseModel<string>> PublishCustomers(RequestModel<List<CustomerDTO>> customers)
        {
            CustomerExchangePublisher.PublishMessage(JsonConvert.SerializeObject(customers));
            return new ResponseModel<string>() { Body = new ResponseBody<string>() { Data = "You request has been submitted successfuly." } };
        }
    }
}
