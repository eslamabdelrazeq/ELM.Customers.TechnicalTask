using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.API.Middleware
{
    public sealed class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(ILogger<RequestResponseLoggingMiddleware> logger, RequestDelegate next)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //https://nblumhardt.com/2016/10/aspnet-core-file-logger/
            //First, get the incoming request

            //We can update the requst timestamp here. but it's not good to interrupt all requests 
            var request = await FormatRequest(context.Request);
            _logger.LogInformation(request);
            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                //Format the response from the server
                var response = await FormatResponse(context.Response);
                _logger.LogInformation(response);
                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return $"{Environment.NewLine}{Environment.NewLine}{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}{Environment.NewLine}{Environment.NewLine}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }


        //public async Task Invoke(HttpContext context)
        //{
        //    logger.LogInformation($"Header: {JsonConvert.SerializeObject(context.Request.Headers, Formatting.Indented)}");

        //    context.Request.EnableBuffering();
        //    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        //    logger.LogInformation($"Body: {body}");
        //    context.Request.Body.Position = 0;

        //    logger.LogInformation($"Host: {context.Request.Host.Host}");
        //    logger.LogInformation($"Client IP: {context.Connection.RemoteIpAddress}");
        //    await next(context);
        //}

    }
}
