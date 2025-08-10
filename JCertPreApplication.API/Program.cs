using JCertPreApplication.API;
using JCertPreApplication.API.Middleware;
using JCertPreApplication.Application;
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
    
    // Bind and validate JWT config
    var jwtConfig = new JwtConfiguration();
    config.GetSection(JwtConfiguration.SectionName).Bind(jwtConfig);
    jwtConfig.Validate();
    
    // Bind and validate Appwrite config
    var appwriteConfig = new AppwriteConfiguration();
    config.GetSection(AppwriteConfiguration.SectionName).Bind(appwriteConfig);
    // Note: Appwrite validation is optional for testing purposes
    try
    {
        appwriteConfig.Validate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Appwrite configuration validation failed: {ex.Message}");
    }
    
    // Bind and validate Firebase config  
    var firebaseConfig = new FirebaseConfiguration();
    config.GetSection(FirebaseConfiguration.SectionName).Bind(firebaseConfig);
    firebaseConfig.Validate();
    
    // Bind and validate LiveKit configuration
    var liveKitConfig = new LiveKitConfiguration();
    config.GetSection("LiveKit").Bind(liveKitConfig);
    liveKitConfig.Validate();
    
    // Bind PayOS configuration
    var payOSConfig = new PayOSConfiguration();
    config.GetSection(PayOSConfiguration.SectionName).Bind(payOSConfig);
    
    // Get API configuration to check if we should show debug info
    var apiConfig = new ApiConfiguration();
    config.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
    
    // Log environment variables for debugging only if enabled
    if (apiConfig.ShowConfigurationStatus)
    {
        LogEnvironmentVariables(config);
    }
    
    // Register all configurations
    builder.Services.Configure<JwtConfiguration>(config.GetSection(JwtConfiguration.SectionName));
    builder.Services.Configure<CorsConfiguration>(config.GetSection(CorsConfiguration.SectionName));
    builder.Services.Configure<ApiConfiguration>(config.GetSection(ApiConfiguration.SectionName));
    builder.Services.Configure<AppwriteConfiguration>(config.GetSection(AppwriteConfiguration.SectionName));
    builder.Services.Configure<FirebaseConfiguration>(config.GetSection(FirebaseConfiguration.SectionName));
    builder.Services.Configure<FrontendConfiguration>(config.GetSection(FrontendConfiguration.SectionName));
    builder.Services.Configure<PayOSConfiguration>(config.GetSection(PayOSConfiguration.SectionName));
    builder.Services.Configure<RedisConfiguration>(config.GetSection("Redis"));

    // Register LiveKit as singleton (already bound and validated above)
    builder.Services.AddSingleton(liveKitConfig);
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
}

#endregion
