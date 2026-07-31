using SmartCity.Core.Utils;
using SmartCity.Domain.Settings;
using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NLog;

namespace SmartCity.Core.Loggers
{
    public class OutRequestDataLogger : IOutRequestDataLogger
    {
        private readonly IOptions<LoggingSettings> _loggingSettings;
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _http;
        public OutRequestDataLogger(IHttpContextAccessor http
            , IOptions<LoggingSettings> loggingSettings)
        {
            _http = http;
            _loggingSettings = loggingSettings;
        }
        public void LogRequest(string request, string response, string endpoint)
        {
            var requestKeys = _loggingSettings.Value.RequestKeysRemove.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            var responseKeys = _loggingSettings.Value.ResponseKeysRemove.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            request = JsonUtils.RemoveKeysByName(request, requestKeys, "***");
            response = JsonUtils.RemoveKeysByName(response, responseKeys, "***");

            var ev = GetLogEvent(request, response, endpoint);
            _logger.Log(ev);
        }

        private LogEventInfo GetLogEvent(string request, string response, string endpoint)
        {
            var eventInfo = new LogEventInfo(LogLevel.Info, "RequestDataLogger", string.Empty);
            eventInfo.Properties["Ip"] = LogProperties.GetIp(_http.HttpContext);
            eventInfo.Properties["Username"] = LogProperties.GetUsername(_http.HttpContext);
            eventInfo.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_http.HttpContext);
            eventInfo.Properties["Url"] = LogProperties.GetUrl(_http.HttpContext);
            eventInfo.Properties["CorrelationID"] = LogProperties.GetCorrelationID(_http.HttpContext);
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            eventInfo.Properties["Request"] = request;
            eventInfo.Properties["Response"] = response;
            eventInfo.Properties["Endpoint"] = endpoint;

            return eventInfo;
        }
    }
}
