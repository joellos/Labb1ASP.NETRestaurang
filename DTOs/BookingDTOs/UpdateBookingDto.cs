using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.BookingDTOs
{
    public class UpdateBookingDto
    {
        public DateTime? BookingDateTime { get; set; }

        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int? NumberOfGuests { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }
    }
}
