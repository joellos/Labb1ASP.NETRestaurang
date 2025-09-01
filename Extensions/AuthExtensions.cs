using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Labb1ASP.NETDatabas.Extensions
{
    public static class AuthExtensions
    {
        /// <summary>
        /// Konfigurerar JWT Authentication
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "RestaurantAPI";
            var audience = jwtSettings["Audience"] ?? "RestaurantClients";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // För development
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,

                    // Claims mapping
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    RoleClaimType = "role"
                };

                // Event handlers för debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Token är validerad - logga för debugging
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        var username = context.Principal?.Identity?.Name;
                        logger.LogInformation("JWT Token validated for user: {Username}", username);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        // Tillåt token från query parameter för vissa endpoints (WebSocket etc.)
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// Konfigurerar Authorization policies
        /// </summary>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Standard administrator policy
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });

                // Policy för att kräva att användaren är autentiserad
                options.AddPolicy("AuthenticatedUser", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });

                // Policy för specifika claims om behövs
                options.AddPolicy("CanManageBookings", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                    // Kan utökas med specifika claims senare
                });

                options.AddPolicy("CanManageMenu", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });

                options.AddPolicy("CanManageTables", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });
            });

            return services;
        }

        /// <summary>
        /// Konfigurerar Identity services om behövs (för lösenordshasning etc.)
        /// </summary>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            // Microsoft.AspNetCore.Identity.PasswordHasher är redan tillgänglig
            // men vi kan konfigurera password requirements här om behövs

            services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 10000; // Standard är bra för säkerhet
            });

            return services;
        }

        /// <summary>
        /// Extension method för att enkelt lägga till alla auth-relaterade services
        /// </summary>
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddJwtAuthentication(configuration)
                .AddAuthorizationPolicies()
                .AddIdentityServices();
        }
    }
}
