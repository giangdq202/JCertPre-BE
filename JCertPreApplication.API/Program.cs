using JCertPreApplication.API;
using JCertPreApplication.Application;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Persistence;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Setup configuration
SetupConfiguration(builder);

// Setup services
SetupServices(builder);

var app = builder.Build();

// Configure pipeline
ConfigurePipeline(app);

// Display configuration status (if enabled)
var apiConfig = new ApiConfiguration();
app.Configuration.GetSection(ApiConfiguration.SectionName).Bind(apiConfig);
if (apiConfig.ShowConfigurationStatus)
{
    DisplayConfigurationStatus(app);
}

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
}

static void ReplaceConfigurationPlaceholders(IConfiguration configuration)
{
    var sections = new[] { "ConnectionStrings", "Jwt", "Cors", "Api" };
    
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
    // Core services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    // Application layers
    builder.Services.AddPersistenceService();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure();
    
    // Database
    SetupDatabase(builder);
    
    // Authentication & Authorization
    SetupAuthentication(builder);
    
    // CORS
    SetupCors(builder);
}

static void SetupDatabase(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("JCertPreDB");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Database connection string is required. Please configure JCERTPRE_DB_CONNECTION_STRING in your .env file.");
    }
    
    builder.Services.AddDbContext<JCertPreDatabaseContext>(options =>
        options.UseNpgsql(connectionString));
}

static void SetupAuthentication(WebApplicationBuilder builder)
{
    var jwtConfig = new JwtConfiguration();
    builder.Configuration.GetSection(JwtConfiguration.SectionName).Bind(jwtConfig);
    
    builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
}

static void SetupCors(WebApplicationBuilder builder)
{
    var corsConfig = new CorsConfiguration();
    builder.Configuration.GetSection(CorsConfiguration.SectionName).Bind(corsConfig);
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins(corsConfig.GetAllowedOriginsArray())
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}
#endregion

#region Pipeline Configuration
static void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowSpecificOrigins");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}

static void DisplayConfigurationStatus(WebApplication app)
{
    var config = app.Configuration;
    var connectionString = config.GetConnectionString("JCertPreDB");
    
    var jwtConfig = new JwtConfiguration();
    config.GetSection(JwtConfiguration.SectionName).Bind(jwtConfig);
    
    var corsConfig = new CorsConfiguration();
    config.GetSection(CorsConfiguration.SectionName).Bind(corsConfig);
    
    Console.WriteLine("\nConfiguration Status:");
    Console.WriteLine("================================");
    
    // Database
    Console.WriteLine($"Database:");
    Console.WriteLine($"   Status: {(!string.IsNullOrEmpty(connectionString) ? "OK Connected" : "ERROR Missing")}");
    if (!string.IsNullOrEmpty(connectionString))
    {
        var maskedConnection = MaskConnectionString(connectionString);
        Console.WriteLine($"   Connection: {maskedConnection}");
    }
    
    // JWT Configuration
    Console.WriteLine($"\nJWT Configuration:");
    Console.WriteLine($"   Status: {(!string.IsNullOrEmpty(jwtConfig.SecretKey) ? "OK Configured" : "ERROR Missing")}");
    Console.WriteLine($"   Issuer: {jwtConfig.Issuer}");
    Console.WriteLine($"   Audience: {jwtConfig.Audience}");
    Console.WriteLine($"   Expiry: {jwtConfig.ExpiryInMinutes} minutes");
    if (!string.IsNullOrEmpty(jwtConfig.SecretKey))
    {
        Console.WriteLine($"   Secret Key: {MaskSecret(jwtConfig.SecretKey)} (Length: {jwtConfig.SecretKey.Length})");
        Console.WriteLine($"   Refresh Key: {MaskSecret(jwtConfig.RefreshSecretKey)} (Length: {jwtConfig.RefreshSecretKey.Length})");
    }
    
    // CORS Configuration
    Console.WriteLine($"\nCORS Configuration:");
    var allowedOrigins = corsConfig.GetAllowedOriginsArray();
    Console.WriteLine($"   Status: {(allowedOrigins.Any() ? "OK Configured" : "ERROR Missing")}");
    if (allowedOrigins.Any())
    {
        Console.WriteLine($"   Origins: [{string.Join(", ", allowedOrigins)}]");
    }
    
    // Environment Variables Check
    Console.WriteLine($"\nEnvironment Variables:");
    var envVars = new[]
    {
        "JCERTPRE_DB_CONNECTION_STRING",
        "JWT_SECRET_KEY", 
        "JWT_REFRESH_SECRET_KEY",
        "JWT_ISSUER",
        "JWT_AUDIENCE", 
        "JWT_EXPIRY_MINUTES",
        "CORS_ALLOWED_ORIGINS",
        "ASPNETCORE_ENVIRONMENT",
        "SHOW_CONFIGURATION_STATUS",
        "REDIS_CONFIGURATION"
    };
    
    foreach (var envVar in envVars)
    {
        var value = Environment.GetEnvironmentVariable(envVar);
        var status = !string.IsNullOrEmpty(value) ? "OK" : "ERROR";
        var displayValue = !string.IsNullOrEmpty(value) ? 
            (envVar.Contains("SECRET") || envVar.Contains("CONNECTION") ? MaskSecret(value) : value) : 
            "Not Set";
        Console.WriteLine($"   {envVar}: {status} {displayValue}");
    }
    
    Console.WriteLine($"\nEnvironment: {app.Environment.EnvironmentName}");
    Console.WriteLine("================================\n");
}

static string MaskSecret(string secret)
{
    if (string.IsNullOrEmpty(secret)) return "Not Set";
    if (secret.Length <= 8) return "***";
    
    return $"{secret[..4]}***{secret[^4..]}";
}

static string MaskConnectionString(string connectionString)
{
    if (string.IsNullOrEmpty(connectionString)) return "Not Set";
    
    // Mask password in connection string
    var masked = connectionString;
    var passwordIndex = masked.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
    if (passwordIndex >= 0)
    {
        var start = passwordIndex + 9; // Length of "Password="
        var end = masked.IndexOf(';', start);
        if (end == -1) end = masked.Length;
        
        var passwordLength = end - start;
        if (passwordLength > 0)
        {
            masked = masked[..start] + new string('*', passwordLength) + masked[end..];
        }
    }
    
    return masked;
}
#endregion
