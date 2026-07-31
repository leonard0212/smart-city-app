namespace SmartCity.Interfaces.Loggers
{
    public interface IApplicationLogger
    {
        void LogInfo(string message);

        void LogWarning(string message);

        void LogError(Exception exception);

        void LogError(Exception exception, string message);
    }
}
