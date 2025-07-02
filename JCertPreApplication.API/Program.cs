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

// Display configuration status (if enabled)
//var apiConfig = new ApiConfiguration();
//app.Configuration.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
//if (apiConfig.ShowConfigurationStatus)
//{
//    DisplayConfigurationStatus(app);
//}

app.Run();

#region Configuration Setup
static void SetupConfiguration(WebApplicationBuilder builder)
{
    LoadEnvironmentVariables();
    builder.Configuration.AddEnvironmentVariables();
    builder.Configuration.AddUserSecrets<Program>();
    
    ReplaceConfigurationPlaceholders(builder.Configuration);
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
    
    // Register all configurations
    builder.Services.Configure<JwtConfiguration>(config.GetSection(JwtConfiguration.SectionName));
    builder.Services.Configure<CorsConfiguration>(config.GetSection(CorsConfiguration.SectionName));
    builder.Services.Configure<ApiConfiguration>(config.GetSection(ApiConfiguration.SectionName));
    builder.Services.Configure<FirebaseConfiguration>(config.GetSection(FirebaseConfiguration.SectionName));
}

static void ReplaceConfigurationPlaceholders(IConfiguration configuration)
{
    var sections = new[] { "ConnectionStrings", "Jwt", "Cors", "Api", "Firebase", "Redis" };
    
    foreach (var sectionName in sections)
    {
        var section = configuration.GetSection(sectionName);
        foreach (var child in section.GetChildren())
        {
            var value = child.Value;
            if (!string.IsNullOrEmpty(value) && value.StartsWith("#{") && value.EndsWith("}#"))
            {
                var envVarName = value[2..^2]; // Equivalent to Substring(2, value.Length - 4)
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                if (!string.IsNullOrEmpty(envValue))
                {
                    configuration[$"{sectionName}:{child.Key}"] = envValue;
                }
            }
        }
    }
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
    // Global Exception Handling - MUST be first in pipeline
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowSpecificOrigins");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    
    // Token revocation check - must be after authentication but before authorization
    app.UseMiddleware<TokenRevocationMiddleware>();
    
    app.UseAuthorization();
    app.MapControllers();
}


#endregion
