using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.AuthDTOs
{
    public class RevokeTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
