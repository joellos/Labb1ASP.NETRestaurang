namespace Labb1ASP.NETDatabas.DTOs.CustomerDTOs
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // För admin-vyer: visa bokningshistorik
        public List<BookingSummaryDto> RecentBookings { get; set; } = new();
    }

    // Förenklad booking-info för customer response
    public class BookingSummaryDto
    {
        public int Id { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int NumberOfGuests { get; set; }
        public int TableNumber { get; set; }
    }
}
