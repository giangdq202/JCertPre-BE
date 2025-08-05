using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Persistence.Cache;
using JCertPreApplication.Persistence.DatabaseContext;
using JCertPreApplication.Persistence.Repositories;
using JCertPreApplication.Persistence.Services;
using JCertPreApplication.Persistence.Services.BackgroudServices;
using JCertPreApplication.Persistence.Services.File;
using JCertPreApplication.Persistence.Services.Firebase;
using JCertPreApplication.Persistence.Services.LiveKit;
using JCertPreApplication.Persistence.Services.Mail;
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
            // Get API configuration to check if we should show status messages
            var apiConfig = new ApiConfiguration();
            configuration.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
            
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
            services.AddScoped<ILivestreamRepository, LivestreamRepository>();
            services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICreditTransactionRepository, CreditTransactionRepository>();

            // Infrastructure Services - Configure both Cloudinary and Appwrite
            // services.AddScoped<IFileService, Services.File.FileService>(); // Cloudinary implementation (commented for testing)
            services.AddScoped<IFileService, Services.File.AppwriteFileService>(); // Appwrite implementation (active for testing)
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<ILiveKitService, Services.LiveKit.LiveKitService>();
            services.AddSingleton<IPasswordService, PasswordService>();

            // Configure Mail Service
            services.Configure<SmtpConfiguration>(configuration.GetSection("Smtp"));
            services.AddScoped<IMailService, MailService>();

            // Configure PayOS
            services.Configure<PayOSConfiguration>(payOSConfig =>
            {
                configuration.GetSection(PayOSConfiguration.SectionName).Bind(payOSConfig);
                
                // Inject BaseUrl từ ApiConfiguration.PublicUrl cho external callbacks
                var apiConfig = configuration.GetSection(ApiConfiguration.SectionName).Get<ApiConfiguration>();
                if (apiConfig != null && !string.IsNullOrEmpty(apiConfig.PublicUrl))
                {
                    payOSConfig.BaseUrl = apiConfig.PublicUrl.TrimEnd('/');
                }
            });
            
            services.AddScoped<IPaymentGateway>(provider =>
            {
                var payOSConfig = configuration.GetSection(PayOSConfiguration.SectionName).Get<PayOSConfiguration>();
                if (payOSConfig == null)
                {
                    throw new ArgumentException("PayOS configuration not found. Please configure PayOS section in appsettings.json");
                }
                
                // Set BaseUrl từ ApiConfiguration.PublicUrl cho external callbacks
                var apiConfig = configuration.GetSection(ApiConfiguration.SectionName).Get<ApiConfiguration>();
                if (apiConfig != null && !string.IsNullOrEmpty(apiConfig.PublicUrl))
                {
                    payOSConfig.BaseUrl = apiConfig.PublicUrl.TrimEnd('/');
                }
                
                return new PayOSService(payOSConfig.ClientId, payOSConfig.ApiKey, payOSConfig.ChecksumKey, 
                    payOSConfig.ReturnUrl, payOSConfig.CancelUrl);
            });

            services.AddBackgroundServices();

            // Show configuration status messages only if enabled
            if (apiConfig.ShowConfigurationStatus)
            {
                Console.WriteLine("✅ Database connection configured successfully");
                Console.WriteLine("✅ Redis cache configured successfully");
                Console.WriteLine("🚀 Appwrite service configured successfully (testing mode)");
                Console.WriteLine("⚠️  Cloudinary service disabled for Appwrite testing");
            }
            
            return services;
        }
        private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddSingleton<TestAttemptAutoSubmitService>();
            services.AddSingleton<ITestAttemptAutoSubmitController>(provider =>
                provider.GetRequiredService<TestAttemptAutoSubmitService>());
            services.AddHostedService<TestAttemptAutoSubmitService>(provider =>
                provider.GetRequiredService<TestAttemptAutoSubmitService>());

            services.AddHostedService<LivestreamStatusBackgroundService>();
            services.AddHostedService<CourseStatusBackgroundService>();

            return services;
        }
    }
}
