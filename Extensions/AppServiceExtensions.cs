using Labb1ASP.NETDatabas.Repositories.Implementations;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Implementations;
using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Extensions
{
    public static class AppServiceExtensions
    {
        /// <summary>
        /// Registrerar alla applikationsspecifika services (Repositories och Services)
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repository registrering
            services.AddScoped<ITableRepository, TableRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAdministratorRepository, AdministratorRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();

            // Service registrering
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddAuthorizationBuilder()
                .AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrator"));



            // Auth service
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        /// <summary>
        /// Konfigurerar CORS för frontend-applikationer
        /// </summary>
        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("RestaurantPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:3000",     // React development server
                            "http://localhost:5173",     // Vite development server
                            "https://localhost:5001",    // MVC app (Uppgift 2)
                            "https://localhost:7001"     // MVC app alternative port
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

                // För development - tillåt alla origins
                options.AddPolicy("Development", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        /// <summary>
        /// Konfigurerar API Controllers med specifika inställningar
        /// </summary>
        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                // Lägg till custom model binding behavior om behövs
                options.SuppressAsyncSuffixInActionNames = false;
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Custom validation response
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    var response = new
                    {
                        Message = "Validation failed",
                        Errors = errors
                    };

                    return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
                };
            });

            return services;
        }

        /// <summary>
        /// Konfigurerar HTTP Client services om behövs för externa API-anrop
        /// </summary>
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            // Exempel: Om vi skulle integrera med externa services
            services.AddHttpClient("ExternalAPI", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }

        /// <summary>
        /// Konfigurerar caching services
        /// </summary>
        public static IServiceCollection AddCachingServices(this IServiceCollection services)
        {
            // Memory cache för att cacha meny och bord-information
            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1000; // Begränsa cache-storlek
            });

            return services;
        }
    }
}
