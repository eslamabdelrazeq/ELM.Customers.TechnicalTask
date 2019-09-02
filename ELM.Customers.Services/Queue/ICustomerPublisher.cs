using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Services.Queue
{
    public interface ICustomerPublisher
    {
        Task<ResponseModel<string>> PublishCustomers(RequestModel<List<CustomerDTO>> customers);

    }
}
