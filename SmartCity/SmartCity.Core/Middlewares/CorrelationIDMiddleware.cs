using Microsoft.AspNetCore.Http;

namespace SmartCity.Core.Middlewares
{
    public class CorrelationIDMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIDMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Items.ContainsKey(AppEnvironment.CorrelationIDKey))
                context.Items.Add(AppEnvironment.CorrelationIDKey, Guid.NewGuid());

            if (!context.Items.ContainsKey(AppEnvironment.RequestStartKey))
                context.Items.Add(AppEnvironment.RequestStartKey, DateTime.Now);

            await _next(context);
        }
    }
}
