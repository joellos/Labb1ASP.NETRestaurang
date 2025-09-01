using Microsoft.OpenApi.Models;

namespace Labb1ASP.NETDatabas.Extensions
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Konfigurerar Swagger med JWT Authentication support
        /// </summary>
        public static IServiceCollection AddSwaggerWithJwtAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant Booking API",
                    Version = "v1",
                    Description = "REST API för restaurangbokningar med JWT-autentisering",
                    Contact = new OpenApiContact
                    {
                        Name = "Restaurant API Team",
                        Email = "support@restaurant.com"
                    }
                });

                // JWT Authentication konfiguration
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Gruppera endpoints logiskt
                c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
                c.DocInclusionPredicate((name, api) => true);
            });

            return services;
        }
    }
}
