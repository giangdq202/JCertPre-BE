using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.Extensions.DependencyInjection;

namespace JCertPreApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}
