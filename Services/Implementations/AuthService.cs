using Labb1ASP.NETDatabas.DTOs.AuthDTOs;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Administrator> _passwordHasher;

        public AuthService(
            IAdministratorRepository administratorRepository,
            IConfiguration configuration)
        {
            _administratorRepository = administratorRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<Administrator>();
        }


        public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
        {
           
            var admin = await _administratorRepository.GetByUsernameAsync(loginDto.Username);
            if (admin == null)
                return null;

           
            var verificationResult = _passwordHasher.VerifyHashedPassword(
                admin, admin.PasswordHash, loginDto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return null;

            
            await _administratorRepository.UpdateLastLoginAsync(admin.Id);

            // Skapa och returnera token response
            return await CreateTokenResponseAsync(admin);
        }

       
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var admin = await VerifyRefreshTokenAsync(request.AdminId, request.RefreshToken);
            if (admin == null)
                return null;

            return await CreateTokenResponseAsync(admin);
        }

        /// <summary>
        /// Återkalla refresh token (logout)
        /// </summary>
        public async Task<bool> RevokeTokenAsync(RevokeTokenRequestDto request)
        {
            // Hitta admin baserat på refresh token
            var admin = await VerifyRefreshTokenFromTokenOnly(request.RefreshToken);
            if (admin == null)
                return false;

            // Rensa refresh token
            await _administratorRepository.UpdateRefreshTokenAsync(
                admin.Id, string.Empty, DateTime.UtcNow);

            return true;
        }

       
        // Validera access token
     
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetTokenValidationParameters();

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

  
        // Skapa komplett token response
      
        private async Task<TokenResponseDto> CreateTokenResponseAsync(Administrator admin)
        {
            var accessToken = GenerateAccessToken(admin);
            var refreshToken = await GenerateAndSaveRefreshTokenAsync(admin);

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                TokenType = "Bearer",
                Administrator = new AdministratorInfoDto
                {
                    Id = admin.Id,
                    Username = admin.Username,
                    Name = admin.Name,
                    Email = admin.Email
                }
            };
        }

  
        private string GenerateAccessToken(Administrator admin)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "RestaurantAPI";
            var audience = jwtSettings["Audience"] ?? "RestaurantClients";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim("role", "Administrator"),
                new Claim("admin_name", admin.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        /// <summary>
        /// Generera och spara refresh token
        /// </summary>
        private async Task<string> GenerateAndSaveRefreshTokenAsync(Administrator admin)
        {
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiryDays = int.Parse(_configuration.GetSection("JwtSettings")["RefreshTokenExpiryDays"] ?? "7");
            var expiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            await _administratorRepository.UpdateRefreshTokenAsync(admin.Id, refreshToken, expiryTime);

            return refreshToken;
        }

        /// <summary>
        /// Generera slumpmässig refresh token
        /// </summary>
        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Verifiera refresh token och returnera administratör
        /// </summary>
        private async Task<Administrator?> VerifyRefreshTokenAsync(Guid adminId, string refreshToken)
        {
            var admin = await _administratorRepository.GetByIdAsync(adminId);

            if (admin == null || admin.RefreshToken != refreshToken)
                return null;

            if (string.IsNullOrEmpty(admin.RefreshToken))
                return null;

            if (admin.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                // Token har gått ut, rensa det
                await _administratorRepository.UpdateRefreshTokenAsync(admin.Id, string.Empty, DateTime.UtcNow);
                return null;
            }

            return admin;
        }

        /// <summary>
        /// Verifiera refresh token utan AdminId (bara för revoke)
        /// </summary>
        private async Task<Administrator?> VerifyRefreshTokenFromTokenOnly(string refreshToken)
        {
            var admins = await _administratorRepository.GetAllAsync();
            var admin = admins.FirstOrDefault(a => a.RefreshToken == refreshToken);

            if (admin == null || string.IsNullOrEmpty(admin.RefreshToken))
                return null;

            if (admin.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                // Token har gått ut, rensa det
                await _administratorRepository.UpdateRefreshTokenAsync(admin.Id, string.Empty, DateTime.UtcNow);
                return null;
            }

            return admin;
        }

        /// <summary>
        /// Hämta token validation parameters
        /// </summary>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "RestaurantAPI";
            var audience = jwtSettings["Audience"] ?? "RestaurantClients";

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };
        }
    }
}