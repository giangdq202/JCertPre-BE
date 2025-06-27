using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Persistence.DatabaseContext;
using JCertPreApplication.Persistence.Repositories;
using JCertPreApplication.Persistence.Services.Firebase;
using JCertPreApplication.Persistence.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JCertPreApplication.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceService(this IServiceCollection services )
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

            services.AddDbContext<JCertPreDatabaseContext>(options =>
                options.UseNpgsql(connectionString));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            
            // Infrastructure Services
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<IPasswordService, PasswordService>();

            Console.WriteLine("✅ Database connection configured successfully");
            return services;
        }
    }
}
