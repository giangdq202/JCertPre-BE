using JCertPreApplication.Application;
using JCertPreApplication.Persistence;

namespace JCertPreApplication.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddPersistenceService(); // Gọi DI từ Persistence
            services.AddApplication(); // Gọi DI từ Application (chỉ chứa interface và logic)
            return services;
        }
    }
}
