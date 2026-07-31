using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestDataLogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, IRequestDataLogger logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            var path = context.Request.Path.Value;

            var requestBodyStream = new MemoryStream();
            var originalRequestBody = context.Request.Body;

            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();

            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;

            var originalBodyStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBodyText = new StreamReader(responseBodyStream).ReadToEnd();
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            context.Request.Body = originalRequestBody;
            context.Response.Body = originalBodyStream;

            _logger.LogRequest(requestBodyText, responseBodyText);

        }
    }

}
