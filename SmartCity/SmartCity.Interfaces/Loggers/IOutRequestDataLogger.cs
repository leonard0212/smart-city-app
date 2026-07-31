namespace SmartCity.Interfaces.Loggers
{
    public interface IOutRequestDataLogger
    {
        void LogRequest(string request, string response, string endpoint);
    }
}
