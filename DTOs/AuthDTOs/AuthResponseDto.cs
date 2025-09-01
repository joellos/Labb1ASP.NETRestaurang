using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;

namespace Labb1ASP.NETDatabas.DTOs.AuthDTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public AdministratorResponseDto Administrator { get; set; } = null!;
    }
}
