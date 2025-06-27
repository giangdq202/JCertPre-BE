using JCertPreApplication.API;
using JCertPreApplication.Persistence;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file FIRST (highest priority)
var root = Directory.GetCurrentDirectory();
// If running from API project folder, go up one level to solution root
if (root.EndsWith("JCertPreApplication.API"))
{
    root = Directory.GetParent(root).FullName;
}
var dotenv = Path.Combine(root, ".env");
Console.WriteLine($"🔍 Looking for .env file at: {dotenv}");

if (File.Exists(dotenv))
{
    DotNetEnv.Env.Load(dotenv);
    Console.WriteLine("✅ Loaded .env file successfully");
    
    // Debug: Check if JWT_SECRET_KEY is loaded
    var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    Console.WriteLine($"🔑 JWT_SECRET_KEY loaded: {(!string.IsNullOrEmpty(jwtKey) ? "YES" : "NO")}");
    
    var dbConnection = Environment.GetEnvironmentVariable("JCERTPRE_DB_CONNECTION_STRING");
    Console.WriteLine($"🗄️ DB Connection loaded: {(!string.IsNullOrEmpty(dbConnection) ? "YES" : "NO")}");
    
    var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
    Console.WriteLine($"🌐 CORS Origins loaded: {(!string.IsNullOrEmpty(corsOrigins) ? "YES" : "NO")}");
}
else
{
    Console.WriteLine("⚠️  .env file not found. Please copy env.example to .env and configure your settings.");
}

// Add configuration sources (env variables have higher priority than appsettings)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container (AFTER loading .env)
builder.Services.AddPersistenceService();
// Add User Secrets
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddInfrastructure();
builder.Services.AddDbContext<JCertPreDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("JCERTPRE_DB_CONNECTION_STRING")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT_ISSUER"],
            ValidAudience = builder.Configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET_KEY"])),
            ClockSkew = TimeSpan.Zero // Đảm bảo thời gian hết hạn chính xác
        };
    });
// Configure CORS
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
                        ?? new[] { "http://localhost:3000", "https://localhost:3000" }; // fallback origins
    
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// Display configuration status (without showing actual values)
Console.WriteLine("🔧 Configuration Status:");
Console.WriteLine($"   Database: {(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JCERTPRE_DB_CONNECTION_STRING")) ? "✅ Configured" : "❌ Missing")}");
Console.WriteLine($"   JWT Secret: {(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")) ? "✅ Configured" : "❌ Missing")}");
Console.WriteLine($"   CORS Origins: {(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")) ? "✅ Configured" : "❌ Missing")}");
Console.WriteLine($"   Environment: {app.Environment.EnvironmentName}");

app.Run();
