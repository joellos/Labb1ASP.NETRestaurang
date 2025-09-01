namespace Labb1ASP.NETDatabas.DTOs.AdministratorDTOs
{
    public class AdministratorResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        
    }
}
