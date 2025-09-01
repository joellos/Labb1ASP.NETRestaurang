using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface ITableService
    {
        Task<IEnumerable<TableResponseDto>> GetAllTablesAsync();
        Task<IEnumerable<TableResponseDto>> GetActiveTablesAsync();
        Task<TableResponseDto?> GetTableByIdAsync(int id);
        Task<TableResponseDto> CreateTableAsync(CreateTableDto tableDto);
        Task<TableResponseDto?> UpdateTableAsync(int id, UpdateTableDto tableDto);
        Task<bool> DeleteTableAsync(int id);
        Task<IEnumerable<TableResponseDto>> GetAvailableTablesAsync(AvailableTablesQueryDto queryDto);
        Task<bool> IsTableNumberAvailableAsync(int tableNumber, int? excludeTableId = null);
    }
}
