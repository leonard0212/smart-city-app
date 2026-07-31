using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;

using NLog;


namespace SmartCity.Core.Loggers
{
    public class DiagnosticsLogger : IDiagnosticsLogger
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _http;

        public DiagnosticsLogger(IHttpContextAccessor http)
        {
            _http = http;
        }

        public void LogInfo(long elapsedMilliseconds)
        {
            LogEvent(LogLevel.Info, elapsedMilliseconds);
        }

        public void LogWarning(long elapsedMilliseconds)
        {
            LogEvent(LogLevel.Warn, elapsedMilliseconds);
        }

        public void LogCritical(long elapsedMilliseconds)
        {
            LogEvent(LogLevel.Fatal, elapsedMilliseconds);
        }

        private LogEventInfo GetLogEvent(LogLevel level, long elapsedMilliseconds)
        {
            var eventInfo = new LogEventInfo(level, "DiagnosticsLogger", string.Empty);

            eventInfo.Properties["Ip"] = LogProperties.GetIp(_http.HttpContext);
            eventInfo.Properties["Username"] = LogProperties.GetUsername(_http.HttpContext);
            eventInfo.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_http.HttpContext);
            eventInfo.Properties["Url"] = LogProperties.GetUrl(_http.HttpContext);
            eventInfo.Properties["ElapsedMilliseconds"] = elapsedMilliseconds.ToString();
            eventInfo.Properties["CorrelationID"] = LogProperties.GetCorrelationID(_http.HttpContext);
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

            return eventInfo;
        }

        private void LogEvent(LogLevel level, long elapsedMilliseconds)
        {
            var ev = GetLogEvent(level, elapsedMilliseconds);
            _logger.Log(ev);
        }
    }
}
