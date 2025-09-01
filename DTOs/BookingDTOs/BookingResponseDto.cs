namespace Labb1ASP.NETDatabas.DTOs.BookingDTOs
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }

        // Nested DTOs för relaterad information
        public CustomerSummaryDto Customer { get; set; } = null!;
        public TableSummaryDto Table { get; set; } = null!;

        // Beräknad property
        public DateTime EndTime => BookingDateTime.AddHours(2);
    }

    // Förenklad kund-info för booking response
    public class CustomerSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    // Förenklad bord-info för booking response
    public class TableSummaryDto
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
    }
}
