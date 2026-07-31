using SmartCity.Domain.Models.Settings;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace SmartCity.Interfaces.Services
{
    public class SystemProperyService : ISystemProperyService
    {

        private readonly IGenericRepositorySimpleLong<SystemProperty> _systemPropertyRepository;

    
        private readonly ICacheService _cacheService;
        public SystemProperyService(IApplicationContext<User> applicationContext
            , IGenericRepositorySimpleLong<SystemProperty> systemPropertyRepository
            , ICacheService cacheService)
        {
            _systemPropertyRepository = systemPropertyRepository;
            _cacheService = cacheService;
        }

        public async Task<string> GetSystemProperyValue(string key)
        {
            var value = await _systemPropertyRepository.QueryAll().Where(x => x.Name == key).FirstOrDefaultAsync();
            return value?.Value;
        }

        public async Task<string> GetSystemProperyValue(string key, int cacheInMinutes)
        {
            var token = _cacheService.GetValue<string>($"_{key}");
            if (token != null)
                return token;

            var tokenValue = await _systemPropertyRepository.QueryAll().Where(x => x.Name == key).FirstOrDefaultAsync();
            _cacheService.SetData<string>($"_{key}", tokenValue?.Value, cacheInMinutes);
            return tokenValue?.Value;
        }

        public async Task<bool> SetSystemProperyValue(string key, string value)
        {
            var property = await _systemPropertyRepository.QueryAll().Where(x => x.Name == key).FirstOrDefaultAsync();
            if (property == null)
                property = new SystemProperty()
                {
                    Name = key,
                    Value = value
                };

            property.Value = value;

            await _systemPropertyRepository.SaveOrUpdateAsync(property);
            await _systemPropertyRepository.CommitChangesAsync();

            return true;
        }
    }
}
