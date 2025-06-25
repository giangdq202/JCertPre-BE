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
            var connectionString = configuration.GetConnectionString("JCertPreDB");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string 'JCertPreDB' not found.");
            }
            services.AddDbContext<JCertPreDatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            // Đăng ký các repository và service


            return services;
        }
    }
}
