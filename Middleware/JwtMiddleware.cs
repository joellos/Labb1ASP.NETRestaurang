using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, authService, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IAuthService authService, string token)
        {
            try
            {
                var isValid = await authService.ValidateTokenAsync(token);
                if (isValid)
                {
                    // Token är giltig - user kommer att sättas via standard JWT middleware
                }
            }
            catch
            {
                // Token validation failed - gör ingenting
                // Request kommer att fortsätta utan user context
            }
        }
    }
}
