using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.CustomerDTOs
{
    public class UpdateCustomerDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
