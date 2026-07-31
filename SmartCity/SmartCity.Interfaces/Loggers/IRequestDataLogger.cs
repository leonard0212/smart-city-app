namespace SmartCity.Interfaces.Loggers
{
    public interface IRequestDataLogger
    {
        void LogRequest(string request, string response);
    }
}
