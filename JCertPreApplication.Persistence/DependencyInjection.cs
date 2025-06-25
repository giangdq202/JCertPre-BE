using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JCertPreApplication.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceService(this IServiceCollection services, IConfiguration configuration)
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

            // Đăng ký các repository và service
            // TODO: Add repository registrations here
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<ICourseRepository, CourseRepository>();

            Console.WriteLine("✅ Database connection configured successfully");
            return services;
        }
    }
}
