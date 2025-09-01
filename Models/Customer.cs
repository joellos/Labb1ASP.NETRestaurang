using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - En kund kan ha många bokningar
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
