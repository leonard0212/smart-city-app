namespace SmartCity.Interfaces.Services.Common
{
    public interface ISystemProperyService
    {
        Task<string> GetSystemProperyValue(string key);
        Task<string> GetSystemProperyValue(string key, int cacheInMinutes);
        Task<bool> SetSystemProperyValue(string key, string value);

    }
}
