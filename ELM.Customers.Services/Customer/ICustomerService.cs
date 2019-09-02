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
        Task<HttpResponseModel<string>> ValidateCustomers(HttpRequestModel<List<CustomerDTO>> customers);
        Task<HttpResponseModel<string>> CreateCustomers(HttpRequestModel<List<CustomerDTO>> customers);
    }
}
