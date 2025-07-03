namespace JCertPreApplication.Application.Features.Cache
{
    public interface ICacheService
    {
        Task<string> GetDataAsync(string id);
    }
}
