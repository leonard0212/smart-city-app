using Microsoft.AspNetCore.Http;

namespace SmartCity.Core.Loggers
{
    public class LogProperties
    {

        public static string GetIp(HttpContext httpContext)
        {
            if (httpContext?.Connection?.RemoteIpAddress == null)
                return null;

            return httpContext.Connection.RemoteIpAddress.ToString();
        }

        public static string GetUsername(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.Name == null)
            {
                if (httpContext?.Items == null)
                    return null;

                var username = httpContext.Items[AppEnvironment.UserIdentityKey] as string;
                return username;
            }

            return httpContext.User.Identity.Name;
        }

        public static string GetHttpMethod(HttpContext httpContext)
        {
            if (httpContext?.Request == null)
                return null;

            return httpContext.Request.Method;
        }

        public static string GetUrl(HttpContext httpContext)
        {
            if (httpContext?.Request == null)
                return null;

            return httpContext.Request.Path;
        }

        public static string GetQueryString(HttpContext httpContext)
        {
            if (httpContext?.Request == null)
                return null;

            return httpContext.Request.QueryString.ToString();
        }

        public static string GetUrlReferer(HttpContext httpContext)
        {
            if (httpContext?.Request?.Headers == null)
                return null;

            var url = httpContext.Request.Headers["Referer"];
            if (!url.Any())
                return null;

            return url.ToString();
        }

        public static Guid? GetCorrelationID(HttpContext httpContext)
        {
            if (!(httpContext?.Items?.ContainsKey(AppEnvironment.CorrelationIDKey) ?? false))
                return null;

            return httpContext.Items[AppEnvironment.CorrelationIDKey] as Guid?;
        }

        public static long? GetRequestElapsedMilliseconds(HttpContext httpContext)
        {
            if (!(httpContext?.Items?.ContainsKey(AppEnvironment.RequestStartKey) ?? false))
                return null;

            return (long?)(DateTime.Now - (httpContext.Items[AppEnvironment.RequestStartKey] as DateTime?))?.TotalMilliseconds;
        }
    }
}
