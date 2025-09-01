using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JCertPreApplication.API.Middleware;
using JCertPreApplication.Domain.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using JCertPreApplication.API.Services;
using JCertPreApplication.Application.Contracts;

namespace JCertPreApplication.API
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures API-specific services including controllers, Swagger, authentication, and CORS.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Core API services
            AddCoreApiServices(services);
            
            // Swagger documentation
            AddSwaggerServices(services);
            
            // Authentication & Authorization
            AddAuthenticationServices(services, configuration);
            
            // CORS
            AddCorsServices(services, configuration);
            
            // Global Exception Handling Middleware
            services.AddScoped<GlobalExceptionHandlingMiddleware>();

            // SignalR Notifier
            services.AddScoped<IChatNotifier, SignalRChatNotifier>();

            return services;
        }

        private static void AddCoreApiServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor(); // Required for accessing HTTP context in services
            services.AddSignalR();
        }

        private static void AddSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "JCertPre API", 
                    Version = "v1.0.0",
                    Description = "API for JCertPre Application - A comprehensive learning and certification platform."
                });
                
                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT Bearer token. Format: Bearer {your-jwt-token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                
                // Group controllers by feature/domain
                c.TagActionsBy(api =>
                {
                    var controllerName = api.ActionDescriptor.RouteValues["controller"];
                    return new[] { controllerName ?? "Unknown" };
                });
                
                // Sort actions alphabetically
                c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
                
                // Enable XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                
                // Include Application layer XML comments if available
                var applicationXmlFile = "JCertPreApplication.Application.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
                if (File.Exists(applicationXmlPath))
                {
                    c.IncludeXmlComments(applicationXmlPath);
                }
            });
        }

        private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = new JwtConfiguration();
            configuration.GetSection(JwtConfiguration.SectionName).Bind(jwtConfig);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
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

        private static void AddCorsServices(IServiceCollection services, IConfiguration configuration)
        {
            var corsConfig = new CorsConfiguration();
            configuration.GetSection(CorsConfiguration.SectionName).Bind(corsConfig);
            
            services.AddCors(options =>
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


    }
}
