using JCertPreApplication.Application;
using JCertPreApplication.Persistence;
<<<<<<< HEAD
=======
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
>>>>>>> Add/redis_config

namespace JCertPreApplication.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistenceService(configuration); // Gọi DI từ Persistence
            services.AddApplication(); // Gọi DI từ Application (chỉ chứa interface và logic)
            return services;
        }
    }
}
