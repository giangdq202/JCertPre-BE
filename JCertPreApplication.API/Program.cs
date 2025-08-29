using JCertPreApplication.API;
using JCertPreApplication.API.Extensions;
using JCertPreApplication.API.Middleware;
using JCertPreApplication.Application;
using JCertPreApplication.API.Hubs;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Setup configuration
SetupConfiguration(builder);

// Setup services
SetupServices(builder);

var app = builder.Build();

// Configure URLs from environment variables or configuration (removed since ASP.NET Core handles this automatically)
// ConfigureAppUrls(app);

// Configure pipeline
ConfigurePipeline(app);

app.Run();

#region Configuration Setup
static void SetupConfiguration(WebApplicationBuilder builder)
{
    LoadEnvironmentVariables();
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddUserSecrets<Program>();
    
    RegisterConfigurations(builder);
}

static void LoadEnvironmentVariables()
{
    var root = Directory.GetCurrentDirectory();
    if (root.EndsWith("JCertPreApplication.API"))
    {
        root = Directory.GetParent(root)!.FullName;
    }
    
    var dotenvPath = Path.Combine(root, ".env");
    if (File.Exists(dotenvPath))
    {
        DotNetEnv.Env.Load(dotenvPath);
        Console.WriteLine("Environment variables loaded");
    }
    else
    {
        Console.WriteLine("WARNING: .env file not found. Please copy env.example to .env");
    }
}

static void RegisterConfigurations(WebApplicationBuilder builder)
{
    var config = builder.Configuration;
    
    // Automatically bind, validate, and register all configurations
    builder.Services.AddValidatedConfiguration<JwtConfiguration>(config);
    builder.Services.AddValidatedConfiguration<AppwriteConfiguration>(config, isValidationOptional: true);
    builder.Services.AddValidatedConfiguration<FirebaseConfiguration>(config);
    builder.Services.AddValidatedConfiguration<LiveKitConfiguration>(config, registerAsSingleton: true); // Restored: needed for direct injection
    builder.Services.AddValidatedConfiguration<PayOSConfiguration>(config);
    builder.Services.AddValidatedConfiguration<CorsConfiguration>(config);
    builder.Services.AddValidatedConfiguration<ApiConfiguration>(config);
    builder.Services.AddValidatedConfiguration<FrontendConfiguration>(config);
    builder.Services.AddValidatedConfiguration<RedisConfiguration>(config);
    builder.Services.AddValidatedConfiguration<SmtpConfiguration>(config);
    
    // Add Gemini AI Configuration with validation
    builder.Services.AddValidatedConfiguration<GeminiConfiguration>(config);
    
    // Get API configuration to check if we should show debug info
    var apiConfig = new ApiConfiguration();
    config.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
    
    // Log environment variables for debugging only if enabled
    if (apiConfig.ShowConfigurationStatus)
    {
        LogEnvironmentVariables(config);
    }
}

static void LogEnvironmentVariables(IConfiguration config)
{
    Console.WriteLine("\n=== ENVIRONMENT VARIABLES DEBUG ===");

    // Group configuration by their root section (e.g., "Jwt", "Api")
    var configGroups = config.AsEnumerable()
        .Where(c => c.Value != null && c.Key.Contains(':')) // We only care about sectioned keys
        .OrderBy(c => c.Key)
        .GroupBy(c => c.Key.Split(':').FirstOrDefault());

    foreach (var group in configGroups)
    {
        if (string.IsNullOrEmpty(group.Key)) continue;

        Console.WriteLine($"\n[{group.Key} Configuration]");
        foreach (var kvp in group)
        {
            // Display the key part (e.g., "SecretKey" instead of "Jwt:SecretKey") and its full value
            var displayKey = kvp.Key.Substring(group.Key.Length + 1);
            Console.WriteLine($"{displayKey}: {kvp.Value ?? "NOT SET"}");
        }
    }

    Console.WriteLine("\n=== END ENVIRONMENT VARIABLES DEBUG ===\n");
}
#endregion

#region Services Setup
static void SetupServices(WebApplicationBuilder builder)
{
    // API layer services (controllers, swagger, authentication, CORS)
    builder.Services.AddApiServices(builder.Configuration);
    
    // Application layer services
    builder.Services.AddApplication();
    
    // Infrastructure layer services (persistence, external services)
    builder.Services.AddInfrastructure(builder.Configuration);
}




#endregion

#region Pipeline Configuration
static void ConfigurePipeline(WebApplication app)
{
    // Global Exception Handling - MUST be first in a pipeline
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    
    // Always enable Swagger. 
    // WARNING: Exposing Swagger UI in production can be a security risk. 
    // It's recommended to secure the endpoint if the API is public.
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("AllowSpecificOrigins");
    // app.UseHttpsRedirection(); // Disabled for HTTP-only deployment with reverse proxy
    app.UseAuthentication();
    
    // Token revocation check - must be after authentication but before authorization
    app.UseMiddleware<TokenRevocationMiddleware>();
    
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<ChatHub>("/hubs/chat");
}

#endregion
