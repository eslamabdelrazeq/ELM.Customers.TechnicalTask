using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Services.Customer
{
    public interface ICustomerService
    {
        Task<ResponseModel<string>> ValidateCustomers(RequestModel<List<CustomerDTO>> customers);
        Task<ResponseModel<string>> CreateCustomers(RequestModel<List<CustomerDTO>> customers);
    }
}
