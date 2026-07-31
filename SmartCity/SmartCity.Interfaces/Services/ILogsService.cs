using SmartCity.Domain.Models.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface ILogsService
    {
        Task<List<MessageBrokerDataLogs>> GetMessageBusLogs();
        Task<MessageBrokerDataLogs> GetMessageBusLogById(long Id);
    }
}
