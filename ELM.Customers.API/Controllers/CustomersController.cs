using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Common.Interfaces.CustomerService;
using ELM.Customers.API.Controllers.Base;
using ELM.Customers.Services.Customer;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ELM.Customers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly IValidator<List<CustomerDTO>> _validator;
        private readonly ICustomerService _customerService;
        private readonly IBus _bus;
        public CustomersController(IValidator<List<CustomerDTO>> validator, IBus bus, ICustomerService customerService)
        {
            _validator = validator;
            _bus = bus;
            _customerService = customerService;
        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] RequestModel<List<CustomerDTO>> customers)
        {
            var result = new ResponseModel<string>();
            if (ModelState.IsValid)
            {
                result = await _customerService.ValidateCustomers(customers);
                if(!result.Body.Errors.Any())
                {
                    await  _bus.Publish<RequestModel<List<CustomerDTO>>>(customers);
                    result.Body.Data = "You request has been submitted successfuly.";
                    return await HandleResponse(customers.Header, result);
                }
            }
            return await HandleResponse(customers.Header,result);
        }
    }
}