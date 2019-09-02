using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELM.Common.BaseRequestResponse;
using ELM.Common.DTO;
using ELM.Notifications.API.Controllers.Base;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ELM.Notifications.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : BaseController
    {
        private readonly IBus _bus;

        public NotificationsController(IBus bus)
        {
            _bus = bus;
        }
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] RequestModel<List<NotificationDTO>> notifications)
        {
            var result = new ResponseModel<string>();
            if (ModelState.IsValid)
            {
                await _bus.Publish<RequestModel<List<NotificationDTO>>>(notifications);
            }
            return await HandleResponse(notifications.Header, result);
        }
    }
}