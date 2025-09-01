using Labb1ASP.NETDatabas.DTOs.AuthDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Autentisera administratör och returnera tokens
        /// </summary>
        Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Förnya access token med refresh token
        /// </summary>
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

        /// <summary>
        /// Återkalla refresh token (logout)
        /// </summary>
        Task<bool> RevokeTokenAsync(RevokeTokenRequestDto request);

        /// <summary>
        /// Validera access token
        /// </summary>
        Task<bool> ValidateTokenAsync(string token);
    }
}
