using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartCity.Core.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class LogFormDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            if (!httpContext.Items.ContainsKey(AppEnvironment.LogFormDataKey))
                httpContext.Items.Add(AppEnvironment.LogFormDataKey, true);
        }
    }
}
