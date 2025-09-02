using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<RestaurantDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Authentication & Authorization  
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            

            // Application Services
            builder.Services.AddApplicationServices();
            builder.Services.AddApiControllers();
            builder.Services.AddCorsServices();
            builder.Services.AddCustomMiddleware();
            builder.Services.AddCachingServices();
            builder.Services.AddHttpClientServices();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithJwtAuth();

            var app = builder.Build();

            // Middleware pipeline
            app.UseRestaurantMiddleware(app.Environment);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
           
            app.Run();
        }
    }
}