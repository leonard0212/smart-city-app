using NLog.Config;
using NLog.Targets;
using NLog;

namespace SmartCity.Core.Loggers
{
    public static class Utils
    {
        public static void ConfigureNLogConnectionString(LoggingConfiguration nlogConfig, string connectionString)
        {
            var dbTargets = nlogConfig.AllTargets.Where(x => x is DatabaseTarget).Cast<DatabaseTarget>().ToList();
            dbTargets.ForEach(dbTarget => dbTarget.ConnectionString = connectionString);

            LogManager.ReconfigExistingLoggers();
        }
    }
}
