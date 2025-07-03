namespace JCertPreApplication.Application.Contracts
{
    public interface ICacheRepository
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task DeleteAsync(string key);
    }
}
