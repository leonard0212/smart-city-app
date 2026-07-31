using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Domain.Models.Logs
{
    public class MessageBrokerDataLogs
    {
        public long Id { get; set; }
        public string? Application { get; set; }
        public DateTime Logged { get; set; }
        public string? QueueName { get; set; }
        public string? Body { get; set; }
        public string? ContentType { get; set; }
        public string? MessageId { get; set; }
        public string? CorrelationId { get; set; }
        public int? DeliveryCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Subject { get; set; }
        public string? To { get; set; } // "[To]" is a reserved word in SQL; in C# it's fine to use as is
        public string? Logger { get; set; }
        public string? SessionId { get; set; }
        public string? EdgeId { get; set; }
    }
}
