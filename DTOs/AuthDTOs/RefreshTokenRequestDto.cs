using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.AuthDTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public Guid AdminId { get; set; } 

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
