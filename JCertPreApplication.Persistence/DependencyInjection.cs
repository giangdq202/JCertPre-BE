using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Persistence.Cache;
using JCertPreApplication.Persistence.DatabaseContext;
using JCertPreApplication.Persistence.Repositories;
using JCertPreApplication.Persistence.Services.Firebase;
using JCertPreApplication.Persistence.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.NameTranslation;

namespace JCertPreApplication.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Read connection string from environment variable only (from .env file)
            var connectionString = Environment.GetEnvironmentVariable("JCERTPRE_DB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(
                    "JCERTPRE_DB_CONNECTION_STRING environment variable not found. " +
                    "Please set it in your .env file. Example: " +
                    "JCERTPRE_DB_CONNECTION_STRING=Host=localhost;Port=5432;Username=admin;Password=yourpassword;Database=JCertPreDB;SSL Mode=Prefer;Trust Server Certificate=true"
                );
            }

            //Configure Database
            services.AddDbContext<JCertPreDatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            // Configure Redis
            services.Configure<RedisConfiguration>(configuration.GetSection("Redis"));
            services.AddSingleton<RedisClient>();

            // Configure Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICacheRepository, RedisCacheRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenCacheRepository, TokenCacheRepository>();
            
            // Infrastructure Services
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddSingleton<IPasswordService, PasswordService>();

            Console.WriteLine("✅ Database connection configured successfully");
            Console.WriteLine("✅ Redis cache configured successfully");
            return services;
        }
    }
}
