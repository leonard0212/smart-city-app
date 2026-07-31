using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using SmartCity.Interfaces.Loggers;

namespace SmartCity.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class StopWatchAttribute : ActionFilterAttribute
    {
        private Stopwatch stopWatch1;
        private Stopwatch stopWatch2;
        private readonly IDiagnosticsLogger _logger;
        private readonly IConfigurationRoot _config;

        public StopWatchAttribute(IDiagnosticsLogger logger, IConfigurationRoot config)
        {
            _logger = logger;
            _config = config;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            stopWatch1 = new Stopwatch();
            stopWatch1.Start();

            stopWatch2 = new Stopwatch();
            stopWatch2.Start();
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (stopWatch1 != null)
            {
                stopWatch1.Stop();

                var controller = context.Controller as Controller;
                if (controller == null)
                    return;

                controller.ViewBag.ElapsedMilliseconds = stopWatch1.ElapsedMilliseconds;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (stopWatch2 != null)
            {
                stopWatch2.Stop();
                var elapsedMilliseconds = stopWatch2.ElapsedMilliseconds;

                if (elapsedMilliseconds > 5000)
                    _logger.LogCritical(elapsedMilliseconds);
                else if (elapsedMilliseconds > 2000)
                    _logger.LogWarning(elapsedMilliseconds);
                else
                    _logger.LogInfo(elapsedMilliseconds);
            }
        }
    }
}
