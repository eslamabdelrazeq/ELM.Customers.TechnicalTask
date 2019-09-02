using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Interfaces.CustomerService;
using ELM.Customers.Database.DAL;

namespace ELM.Customers.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<ELM.Common.Entities.Customer> _customerRepository;
        public CustomerService(IGenericRepository<ELM.Common.Entities.Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ResponseModel<string>> ValidateCustomers(RequestModel<List<CustomerDTO>> customers)
        {
            //Sample business validation
            var result = new ResponseModel<string>() { Body = new ResponseBody<string>() { Errors = new List<string>()} };
            var emails = customers.Body.Select(d => d.Email).ToList();
            GridOptions<ELM.Common.Entities.Customer> gridOptions = new GridOptions<ELM.Common.Entities.Customer>();
            gridOptions.AddFilter(c => emails.Contains(c.Email));
            var repeatedEmails = await _customerRepository.ReadAll(gridOptions: gridOptions);
            if (repeatedEmails.Any())
            {
                result.Body.Errors = new List<string>();
                foreach (var mail in repeatedEmails)
                {
                    result.Body.Errors.Add($"The following email is already in use {mail.Email}");
                }
            }
            return result;
        }
        public async Task<ResponseModel<string>> CreateCustomers(RequestModel<List<CustomerDTO>> customers)
        {
            //Sample business validation
            var result = new ResponseModel<string>() { Body = new ResponseBody<string>() { Data = "You request has been added successfuly to database." } };
            var customersList = customers.Body.Select(d => new ELM.Common.Entities.Customer
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                Email = d.Email,
                Phone = d.Phone,
            });
            await _customerRepository.Create(customersList.ToList());
            return result;
        }
    }
}
