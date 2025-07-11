using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Features.Enrollment;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Features.LiveKit;
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
            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IInstructorProfileService, InstructorProfileService>();
            services.AddScoped<IStudentProfileService, StudentProfileService>();
            services.AddScoped<IStudyPlanService, StudyPlanService>();
            services.AddScoped<IStudyPlanItemService, StudyPlanItemService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IChoiceService, ChoiceService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ILiveKitService, LiveKitService>();
            services.AddScoped<ISubContentService, SubContentService>();
            services.AddScoped<ITestQuestionService, TestQuestionService>();

            return services;
        }
    }
}
