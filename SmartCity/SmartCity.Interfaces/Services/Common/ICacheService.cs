namespace SmartCity.Interfaces.Services.Common
{
    public interface ICacheService
    {
        void Remove(string key);
        void SetData<TEntity>(string key, TEntity value, int expiresIn) where TEntity : class;
        TEntity GetValue<TEntity>(string key) where TEntity : class;
    }
}
