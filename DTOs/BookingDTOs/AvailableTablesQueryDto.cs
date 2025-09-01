using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.BookingDTOs
{
    public class AvailableTablesQueryDto
    {
        [Required]
        public DateTime BookingDateTime { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int NumberOfGuests { get; set; }
    }
}
