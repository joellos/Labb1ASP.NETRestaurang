using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.AdministratorDTOs
{
    public class UpdateAdministratorDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string? Username { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
