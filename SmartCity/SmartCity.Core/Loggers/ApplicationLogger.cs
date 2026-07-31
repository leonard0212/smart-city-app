using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;
using NLog;

namespace SmartCity.Core.Loggers
{
    public class ApplicationLogger : IApplicationLogger
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IHttpContextAccessor _http;

        public ApplicationLogger(IHttpContextAccessor http)
        {
            _http = http;
        }

        public void LogError(Exception exception)
        {
            LogError(exception, exception.Message);
        }

        public void LogError(Exception exception, string message)
        {
            LogEvent(LogLevel.Error, message, exception);
        }

        public void LogInfo(string message)
        {
            LogEvent(LogLevel.Info, message);
        }

        public void LogWarning(string message)
        {
            LogEvent(LogLevel.Warn, message);
        }

        private void LogEvent(LogLevel level, string message, Exception exception = null)
        {
            var ev = GetLogEvent(level, message, exception);
            Logger.Log(ev);
        }

        private LogEventInfo GetLogEvent(LogLevel level, string message, Exception ex = null)
        {
            var eventInfo = new LogEventInfo(level, "ApplicationLogger", message);
            eventInfo.Exception = ex;

            eventInfo.Properties["Ip"] = LogProperties.GetIp(_http.HttpContext);
            eventInfo.Properties["Username"] = LogProperties.GetUsername(_http.HttpContext);
            eventInfo.Properties["CorrelationID"] = LogProperties.GetCorrelationID(_http.HttpContext);
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

            return eventInfo;
        }

    }
}
