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
    
    // Bind and validate Cloudinary config
    var cloudinaryConfig = new CloudinaryConfiguration();
    config.GetSection(CloudinaryConfiguration.SectionName).Bind(cloudinaryConfig);
    cloudinaryConfig.Validate();
    
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
    builder.Services.Configure<CloudinaryConfiguration>(config.GetSection(CloudinaryConfiguration.SectionName));
    builder.Services.Configure<AppwriteConfiguration>(config.GetSection(AppwriteConfiguration.SectionName));
    builder.Services.Configure<FirebaseConfiguration>(config.GetSection(FirebaseConfiguration.SectionName));

    // Register and validate LiveKit configuration
    var liveKitConfig = new LiveKitConfiguration();
    config.GetSection("LiveKit").Bind(liveKitConfig);
    liveKitConfig.Validate();
    builder.Services.AddSingleton(liveKitConfig);
}

static void LogEnvironmentVariables(IConfiguration config)
{
    Console.WriteLine("\n=== ENVIRONMENT VARIABLES DEBUG ===");
    
    // Database Configuration
    Console.WriteLine($"Database ConnectionString: {MaskSensitiveData(config.GetConnectionString("JCertPreDB"))}");
    
    // JWT Configuration
    Console.WriteLine("\n[JWT Configuration]");
    Console.WriteLine($"SecretKey: {MaskSensitiveData(config["Jwt:SecretKey"])}");
    Console.WriteLine($"RefreshSecretKey: {MaskSensitiveData(config["Jwt:RefreshSecretKey"])}");
    Console.WriteLine($"Issuer: {config["Jwt:Issuer"]}");
    Console.WriteLine($"Audience: {config["Jwt:Audience"]}");
    Console.WriteLine($"ExpiryInMinutes: {config["Jwt:ExpiryInMinutes"]}");
    
    // Redis Configuration
    Console.WriteLine("\n[Redis Configuration]");
    Console.WriteLine($"ConnectionString: {MaskSensitiveData(config["Redis:ConnectionString"])}");
    
    // API Configuration
    Console.WriteLine("\n[API Configuration]");
    Console.WriteLine($"Environment: {config["Api:Environment"]}");
    Console.WriteLine($"Urls: {config["Api:Urls"]}");
    Console.WriteLine($"ShowConfigurationStatus: {config["Api:ShowConfigurationStatus"]}");
    
    // CORS Configuration
    Console.WriteLine("\n[CORS Configuration]");
    Console.WriteLine($"AllowedOrigins: {config["Cors:AllowedOrigins"]}");
    
    // Cloudinary Configuration
    Console.WriteLine("\n[Cloudinary Configuration]");
    Console.WriteLine($"CloudName: {config["Cloudinary:CloudName"]}");
    Console.WriteLine($"ApiKey: {MaskSensitiveData(config["Cloudinary:ApiKey"])}");
    Console.WriteLine($"ApiSecret: {MaskSensitiveData(config["Cloudinary:ApiSecret"])}");
    Console.WriteLine($"Secure: {config["Cloudinary:Secure"]}");
    
    // Appwrite Configuration
    Console.WriteLine("\n[Appwrite Configuration]");
    Console.WriteLine($"Endpoint: {config["Appwrite:Endpoint"]}");
    Console.WriteLine($"ProjectId: {config["Appwrite:ProjectId"]}");
    Console.WriteLine($"ApiKey: {MaskSensitiveData(config["Appwrite:ApiKey"])}");
    Console.WriteLine($"ImagesBucketId: {config["Appwrite:ImagesBucketId"]}");
    Console.WriteLine($"VideosBucketId: {config["Appwrite:VideosBucketId"]}");
    Console.WriteLine($"DocumentsBucketId: {config["Appwrite:DocumentsBucketId"]}");
    Console.WriteLine($"MaxFileSizeMB: {config["Appwrite:MaxFileSizeMB"]}");
    
    // Firebase Configuration
    Console.WriteLine("\n[Firebase Configuration]");
    Console.WriteLine($"Type: {config["Firebase:Type"]}");
    Console.WriteLine($"ProjectId: {config["Firebase:ProjectId"]}");
    Console.WriteLine($"PrivateKeyId: {MaskSensitiveData(config["Firebase:PrivateKeyId"])}");
    Console.WriteLine($"PrivateKey: {(string.IsNullOrEmpty(config["Firebase:PrivateKey"]) ? "NOT SET" : "***MASKED***")}");
    Console.WriteLine($"ClientEmail: {config["Firebase:ClientEmail"]}");
    Console.WriteLine($"ClientId: {config["Firebase:ClientId"]}");
    Console.WriteLine($"AuthUri: {config["Firebase:AuthUri"]}");
    Console.WriteLine($"TokenUri: {config["Firebase:TokenUri"]}");
    
    // LiveKit Configuration
    Console.WriteLine("\n[LiveKit Configuration]");
    Console.WriteLine($"ApiKey: {MaskSensitiveData(config["LiveKit:ApiKey"])}");
    Console.WriteLine($"ApiSecret: {MaskSensitiveData(config["LiveKit:ApiSecret"])}");
    Console.WriteLine($"ServerUrl: {config["LiveKit:ServerUrl"]}");
    
    Console.WriteLine("\n=== END ENVIRONMENT VARIABLES DEBUG ===\n");
}

static string MaskSensitiveData(string? value)
{
    if (string.IsNullOrEmpty(value))
        return "NOT SET";
    
    if (value.Length <= 4)
        return "***";
    
    return $"{value.Substring(0, 4)}***{value.Substring(value.Length - 4)}";
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
