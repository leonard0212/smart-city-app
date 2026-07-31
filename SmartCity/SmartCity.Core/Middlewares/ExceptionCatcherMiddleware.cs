using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;

namespace SmartCity.Core.Middlewares
{
    public class ExceptionCatcherMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionCatcherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IApplicationLogger logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}
