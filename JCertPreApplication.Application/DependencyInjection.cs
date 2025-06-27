<<<<<<< HEAD
﻿using JCertPreApplication.Application.Features.Auth;
=======
﻿using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using Microsoft.Extensions.Configuration;
>>>>>>> Add/redis_config
using Microsoft.Extensions.DependencyInjection;

namespace JCertPreApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}
