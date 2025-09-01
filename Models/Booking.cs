using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public DateTime BookingDateTime { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
        public int NumberOfGuests { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Table Table { get; set; } = null!;

        // Beräknad property för slutdatum (2 timmar från start)
        public DateTime EndTime => BookingDateTime.AddHours(2);
    }
}
