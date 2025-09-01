using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.BookingDTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int TableId { get; set; }

        [Required]
        public DateTime BookingDateTime { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int NumberOfGuests { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? SpecialRequests { get; set; }
    }
}
