using System.ComponentModel.DataAnnotations;

namespace Labb1ASP.NETDatabas.DTOs.TableDTOs
{
    public class CreateTableDto
    {
        [Required]
        [Range(1, 999, ErrorMessage = "Table number must be between 1 and 999")]
        public int TableNumber { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
        public int Capacity { get; set; }
    }
}
