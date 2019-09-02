using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Customers.Producer;
using ELM.Notifications.API.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ELM.Notifications.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseController
    {
        public NotificationsController()
        {

        }
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] HttpRequestModel<List<CustomerDTO>> customers)
        {
            var result = new HttpResponseModel<string>();
            if (ModelState.IsValid)
            {
               NotificationExchangePublisher.PublishMessage(JsonConvert.SerializeObject(customers));
            }
            return await HandleResponse(customers.Header, result);
        }
    }
}