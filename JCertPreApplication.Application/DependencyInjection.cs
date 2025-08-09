using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.AdminDashboard;
using JCertPreApplication.Application.Features.AttemptAnswers;
using JCertPreApplication.Application.Features.Auth;
using JCertPreApplication.Application.Features.Cache;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Features.Documents;
using JCertPreApplication.Application.Features.Enrollment;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Features.LessonProgresses;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Features.Livestreams;
using JCertPreApplication.Application.Features.Payment;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Application.Features.StudentProfile;
using JCertPreApplication.Application.Features.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Application.Features.Tests;
using JCertPreApplication.Application.Features.TestTemplateConfigs;
using JCertPreApplication.Application.Features.Users;
using Microsoft.Extensions.DependencyInjection;

namespace JCertPreApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
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
            services.AddScoped<ISubContentService, SubContentService>();
            services.AddScoped<ITestQuestionService, TestQuestionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITestAttemptService, TestAttemptService>();
            services.AddScoped<IAttemptAnswerService, AttemptAnswerService>();
            services.AddScoped<ITestTemplateTypeService, TestTemplateTypeService>();
            services.AddScoped<ITestTemplateService, TestTemplateService>();
            services.AddScoped<ITestTemplateConfigService, TestTemplateConfigService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<ILivestreamService, LivestreamService>();
            services.AddScoped<ILessonProgressService, LessonProgressService>();
            services.AddScoped<IPaymentService, PaymentService>();

            // Add AutoMapper
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
