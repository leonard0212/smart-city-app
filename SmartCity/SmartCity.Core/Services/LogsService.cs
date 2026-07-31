using Microsoft.EntityFrameworkCore;
using SmartCity.Database;
using SmartCity.Domain.Models.Logs;
using SmartCity.Interfaces.Services;

namespace SmartCity.Core.Services
{
    public class LogsService : ILogsService
    {
        private readonly LogDatabaseContext _logDatabaseContext;
        public LogsService(LogDatabaseContext logDatabaseContext)
        {
            _logDatabaseContext = logDatabaseContext;
        }




        public async Task<List<MessageBrokerDataLogs>> GetMessageBusLogs()
        {
            var result = await _logDatabaseContext.MessageBrokerDataLogs.OrderByDescending(x => x.Logged).Take(20).ToListAsync();
            return result;
        }


        public async Task<MessageBrokerDataLogs> GetMessageBusLogById(long Id)
        {
            var result = await _logDatabaseContext.MessageBrokerDataLogs.FirstOrDefaultAsync(x => x.Id == Id);
            return result;
        }

    }
}
