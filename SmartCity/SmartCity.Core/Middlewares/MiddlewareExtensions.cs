using Microsoft.AspNetCore.Builder;
namespace SmartCity.Core.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }

        public static IApplicationBuilder UseSetupCorrelationID(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIDMiddleware>();
        }

        public static IApplicationBuilder UseExceptionCatcher(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionCatcherMiddleware>();
        }
    }
}
