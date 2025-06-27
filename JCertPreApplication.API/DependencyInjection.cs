using JCertPreApplication.Application;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
