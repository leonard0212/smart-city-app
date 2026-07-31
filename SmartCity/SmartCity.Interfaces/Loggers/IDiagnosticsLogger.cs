namespace SmartCity.Interfaces.Loggers
{
    public interface IDiagnosticsLogger
    {
        void LogInfo(long elapsedMilliseconds);

        void LogWarning(long elapsedMilliseconds);

        void LogCritical(long elapsedMilliseconds);
    }
}
