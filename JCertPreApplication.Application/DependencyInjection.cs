using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Application.Features.StudentProfile;
using JCertPreApplication.Application.Features.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Application.Features.Tests;
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
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IInstructorProfileService, InstructorProfileService>();
            services.AddScoped<IStudentProfileService, StudentProfileService>();
            services.AddScoped<IStudyPlanService, StudyPlanService>();
            services.AddScoped<IStudyPlanItemService, StudyPlanItemService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IChoiceService, ChoiceService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ISubContentService, SubContentService>();
            return services;
        }
    }
}
