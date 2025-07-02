using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.Extensions.DependencyInjection;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Features.StudentProfile;

namespace JCertPreApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IInstructorProfileService, InstructorProfileService>();
            services.AddScoped<IStudentProfileService, StudentProfileService>();
            return services;
        }
    }
}
