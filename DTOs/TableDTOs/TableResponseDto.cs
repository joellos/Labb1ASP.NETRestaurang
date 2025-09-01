namespace Labb1ASP.NETDatabas.DTOs.TableDTOs
{
    public class TableResponseDto
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }

        // För admin-vyer: visa antal aktuella bokningar
        public int CurrentBookingsCount { get; set; }
    }
}
