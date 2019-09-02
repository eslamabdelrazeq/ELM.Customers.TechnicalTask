using ELM.Common.BaseRequestResponse;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELM.Customers.API.Controllers.Base
{
    public class BaseController : ControllerBase
    {
        public async Task<IActionResult> HandleResponse<T>(BaseRequestResponseHeader header, ResponseModel<T> result)
        {
            //This part also could be added to middleware but it's better here to avoid desiralization in the middleware.
            if (result.Body is null)
            {
                result = new ResponseModel<T>();
            }
            result.Header = header;
            result.Header.timeStamp = DateTime.Now;
            if (result.Body is null)
            {
                result.Body = new ResponseBody<T>();
            }
            if (result.Body.Errors is null)
            {
                result.Body.Errors = new List<string>();
            }
            if (!ModelState.IsValid || result.Body.Errors.Any())
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage);
                result.Body.Errors.AddRange(errors.ToList());
                return BadRequest(result);
            }
            return Ok(result);
        }

        public async Task<IActionResult> EmptySuccess<T>(BaseRequestResponseHeader header, ResponseModel<T> result)
        {
            return null;
        }
    }
}
