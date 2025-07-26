using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Persistence.Cache;
using JCertPreApplication.Persistence.DatabaseContext;
using JCertPreApplication.Persistence.Repositories;
using JCertPreApplication.Persistence.Services.Cloudinary;
using JCertPreApplication.Persistence.Services.Firebase;
using JCertPreApplication.Persistence.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.NameTranslation;

namespace JCertPreApplication.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Read connection string using .NET Configuration system (supports environment variables with ConnectionStrings__JCertPreDB format)
            var connectionString = configuration.GetConnectionString("JCertPreDB");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(
                    "JCertPreDB connection string not found. " +
                    "Please set it using environment variable: ConnectionStrings__JCertPreDB or in appsettings.json"
                );
            }

            //Configure Database
            services.AddDbContext<JCertPreDatabaseContext>(options =>
                options.UseNpgsql(connectionString));

            // Configure Redis
            services.Configure<RedisConfiguration>(configuration.GetSection("Redis"));
            services.AddSingleton<RedisClient>();

            // Configure Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ICacheRepository, RedisCacheRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ITokenCacheRepository, TokenCacheRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<IInstructorProfileRepository, InstructorProfileRepository>();
            services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
            services.AddScoped<IStudyPlanRepository, StudyPlanRepository>();
            services.AddScoped<IStudyPlanItemRepository, StudyPlanItemRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IChoiceRepository, ChoiceRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<ICourseInstructorRepository, CourseInstructorRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<ISubContentRepository, SubContentRepository>();
            services.AddScoped<ITestQuestionRepository, TestQuestionRepository>();
            services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
            services.AddScoped<IAttemptAnswerRepository, AttemptAnswerRepository>();
            services.AddScoped<ITestScoreSummaryRepository, TestScoreSummaryRepository>();
            services.AddScoped<ITestTemplateTypeRepository, TestTemplateTypeRepository>();
            services.AddScoped<ITestTemplateRepository, TestTemplateRepository>();
            services.AddScoped<ITestTemplateConfigRepository, TestTemplateConfigRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            // Infrastructure Services
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddSingleton<IPasswordService, PasswordService>();

            services.AddBackgroundServices();

            Console.WriteLine("✅ Database connection configured successfully");
            Console.WriteLine("✅ Redis cache configured successfully");
            Console.WriteLine("✅ Cloudinary service configured successfully");
            return services;
        }
        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<TestAttemptAutoSubmitService>();
            services.AddSingleton<ITestAttemptAutoSubmitController>(provider =>
                provider.GetRequiredService<TestAttemptAutoSubmitService>());
            services.AddHostedService<TestAttemptAutoSubmitService>(provider =>
                provider.GetRequiredService<TestAttemptAutoSubmitService>());

            return services;
        }
    }
}
