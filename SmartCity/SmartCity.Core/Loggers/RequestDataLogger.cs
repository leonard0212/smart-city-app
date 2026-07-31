using SmartCity.Core.Utils;
using SmartCity.Interfaces.Loggers;
using Microsoft.AspNetCore.Http;
using NLog;

namespace SmartCity.Core.Loggers
{
    public class RequestDataLogger : IRequestDataLogger
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpContextAccessor _http;

        public RequestDataLogger(IHttpContextAccessor http)
        {
            _http = http;

        }
        public void LogRequest(string request, string response)
        {
            string[] requestKeys = { "password" };
            string[] responseKeys = { "base64Content" };
            //var requestKeys = "password";// _loggingSettings.Value?.RequestKeysRemove?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //  var responseKeys = "base64Content"; //_loggingSettings.Value?.ResponseKeysRemove?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            request = JsonUtils.RemoveKeysByName(request, requestKeys, "***");
            response = JsonUtils.RemoveKeysByName(response, responseKeys, "***");

            var ev = GetLogEvent(request, response);
            _logger.Log(ev);
        }

        private LogEventInfo GetLogEvent(string request, string response)
        {
            //se scos de aici daca nu ne trebuie
            //var documentUid = JsonUtils.GetValueByKey(response, "offerCode");

            var eventInfo = new LogEventInfo(LogLevel.Info, "RequestDataLogger", string.Empty);
            eventInfo.Properties["Ip"] = LogProperties.GetIp(_http.HttpContext);
            eventInfo.Properties["Username"] = LogProperties.GetUsername(_http.HttpContext);
            eventInfo.Properties["HttpMethod"] = LogProperties.GetHttpMethod(_http.HttpContext);
            eventInfo.Properties["Url"] = LogProperties.GetUrl(_http.HttpContext);
            eventInfo.Properties["CorrelationID"] = LogProperties.GetCorrelationID(_http.HttpContext);
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            eventInfo.Properties["Request"] = request;
            eventInfo.Properties["Response"] = response;
            //eventInfo.Properties["DocumentUid"] = documentUid;



            return eventInfo;
        }

    }
}
