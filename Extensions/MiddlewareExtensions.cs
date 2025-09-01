using Labb1ASP.NETDatabas.Middleware;
using RestaurantBookingAPI.Middleware;

namespace Labb1ASP.NETDatabas.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Konfigurerar alla middleware i rätt ordning
        /// </summary>
        public static IApplicationBuilder UseRestaurantMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 1. Exception handling (måste vara först)
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // 2. Global exception handling middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // 3. HTTPS Redirection
            app.UseHttpsRedirection();

            // 4. Request logging middleware
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 5. CORS (före authentication)
            app.UseCors();

            // 6. Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        /// <summary>
        /// Registrerar custom middleware services
        /// </summary>
        public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
        {
            // Registrera middleware dependencies om behövs
            services.AddScoped<RequestLoggingService>();

            return services;
        }
    }

    // Service för request logging
    public class RequestLoggingService
    {
        private readonly ILogger<RequestLoggingService> _logger;

        public RequestLoggingService(ILogger<RequestLoggingService> logger)
        {
            _logger = logger;
        }

        public void LogRequest(HttpContext context, TimeSpan duration)
        {
            _logger.LogInformation(
                "Request {Method} {Path} responded {StatusCode} in {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
    }
}
