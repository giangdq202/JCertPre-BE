using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    "JCERTPRE_DB_CONNECTION_STRING=Server=localhost;Database=JCertPreDB;User ID=sa;Password=yourpassword;TrustServerCertificate=True"
                );
            }

            services.AddDbContext<JCertPreDatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            // Đăng ký các repository và service
            // TODO: Add repository registrations here
            // services.AddScoped<IUserRepository, UserRepository>();
            // services.AddScoped<ICourseRepository, CourseRepository>();

            Console.WriteLine("✅ Database connection configured successfully");
            return services;
        }
    }
}
