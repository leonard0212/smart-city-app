using SmartCity.Core.Extensions;
using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;
using NLog;

namespace SmartCity.Core.Loggers
{
    public class WebLogger : IWebLogger
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _http;

        public WebLogger(IHttpContextAccessor http)
        {
            _http = http;
        }

        public void LogRequest()
        {
            var ev = GetLogEvent();
            _logger.Log(ev);
        }

        private LogEventInfo GetLogEvent()
        {
            var eventInfo = new LogEventInfo(LogLevel.Info, "RequestHistoryLogger", string.Empty);
            eventInfo.Properties["Ip"] = LogProperties.GetIp(_http.HttpContext);
            eventInfo.Properties["Username"] = LogProperties.GetUsername(_http.HttpContext);
            eventInfo.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_http.HttpContext);
            eventInfo.Properties["Url"] = LogProperties.GetUrl(_http.HttpContext);
            eventInfo.Properties["QueryString"] = LogProperties.GetQueryString(_http.HttpContext);
            eventInfo.Properties["UrlReferrer"] = LogProperties.GetUrlReferer(_http.HttpContext);
            eventInfo.Properties["CorrelationID"] = LogProperties.GetCorrelationID(_http.HttpContext);
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");

            if (_http.HttpContext.Request.HasFormContentType && _http.HttpContext.Items.ContainsKey(AppEnvironment.LogFormDataKey))
                eventInfo.Properties["FormData"] = _http.HttpContext.Request.Form.GetFormDataSerialized();

            return eventInfo;
        }
    }
}
