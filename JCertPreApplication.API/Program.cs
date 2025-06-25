using JCertPreApplication.Persistence;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file FIRST (highest priority)
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
if (File.Exists(dotenv))
{
    DotNetEnv.Env.Load(dotenv);
    Console.WriteLine("✅ Loaded .env file successfully");
}
else
{
    Console.WriteLine("⚠️  .env file not found. Please copy env.example to .env and configure your settings.");
}

// Add configuration sources (env variables have higher priority than appsettings)
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddPersistenceService(builder.Configuration);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173"
            )
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
Console.WriteLine($"   Environment: {app.Environment.EnvironmentName}");

app.Run();
