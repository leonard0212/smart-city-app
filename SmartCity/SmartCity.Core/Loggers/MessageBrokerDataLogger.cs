using Azure.Core;
using Azure.Messaging.ServiceBus;
using SmartCity.Core.Utils;
using SmartCity.Interfaces.Loggers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using SmartCity.Domain.ServiceModels.RawDetection;
using System.Text.Json;

namespace SmartCity.Core.Loggers
{
    public class MessageBrokerDataLogger : IMessageBrokerDataLogger
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public void LogRequest(ServiceBusReceivedMessage message, string queueName)
        {

            var ev = GetLogEvent(message);
            ev.Properties["QueueName"] = queueName;
            _logger.Log(ev);
        }

        private LogEventInfo GetLogEvent(ServiceBusReceivedMessage message)
        {
            var edgeId = JsonUtils.GetValueByKey(message?.Body?.ToString(), "EdgeId");

            var eventInfo = new LogEventInfo(LogLevel.Info, "MessageBrokerDataLogger", string.Empty);
            eventInfo.Properties["Body"] = message?.Body?.ToString();
            eventInfo.Properties["EdgeId"] = edgeId;
            eventInfo.Properties["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            eventInfo.Properties["ContentType"] = message?.ContentType;
            eventInfo.Properties["MessageId"] = message?.MessageId;
            eventInfo.Properties["CorrelationId"] = message?.CorrelationId;
            eventInfo.Properties["DeliveryCount"] = message?.DeliveryCount;
            eventInfo.Properties["ExpiresAt"] = message?.ExpiresAt.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
            eventInfo.Properties["Subject"] = message?.Subject;
            eventInfo.Properties["To"] = message?.To;
            eventInfo.Properties["SessionId"] = message?.SessionId;

            return eventInfo;
        }
    }
}
