using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Loggers
{
    public interface IMessageBrokerDataLogger
    {
        void LogRequest(ServiceBusReceivedMessage request, string queueName);
    }
}
