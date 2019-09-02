using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Customers.API.Controllers.Base;
using ELM.Customers.Services.Customer;
using ELM.Customers.Services.Queue;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELM.Customers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly IValidator<List<CustomerDTO>> _validator;
        private readonly ICustomerPublisher _customerPublisher;
        private readonly ICustomerService _customerService;
        public CustomersController(IValidator<List<CustomerDTO>> validator, ICustomerService customerService, ICustomerPublisher customerPublisher)
        {
            _customerPublisher = customerPublisher;
            _customerService = customerService;
            _validator = validator;
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] HttpRequestModel<List<CustomerDTO>> customers)
        {
            var result = new HttpResponseModel<string>();
            if (ModelState.IsValid)
            {
                result = await _customerService.ValidateCustomers(customers);
                if(!result.Body.Errors.Any())
                {
                    result = await _customerPublisher.PublishCustomers(customers);
                    return await HandleResponse(customers.Header, result);
                }
            }
            return await HandleResponse(customers.Header,result);
        }
    }
}