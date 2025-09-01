using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Extensions;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepository;
        private readonly IBookingRepository _bookingRepository;

        public TableService(ITableRepository tableRepository, IBookingRepository bookingRepository)
        {
            _tableRepository = tableRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<TableResponseDto>> GetAllTablesAsync()
        {
            var tables = await _tableRepository.GetAllAsync();
            return tables.Select(t => t.ToResponseDto());
        }

        public async Task<IEnumerable<TableResponseDto>> GetActiveTablesAsync()
        {
            var tables = await _tableRepository.GetActiveTablesAsync();
            return tables.Select(t => t.ToResponseDto());
        }

        public async Task<TableResponseDto?> GetTableByIdAsync(int id)
        {
            var table = await _tableRepository.GetByIdAsync(id);
            return table?.ToResponseDto();
        }

        public async Task<TableResponseDto> CreateTableAsync(CreateTableDto tableDto)
        {
            if (await _tableRepository.TableNumberExistsAsync(tableDto.TableNumber))
                throw new InvalidOperationException($"Table number {tableDto.TableNumber} already exists.");

            var table = tableDto.ToEntity();
            var created = await _tableRepository.CreateAsync(table);
            return created.ToResponseDto();
        }

        public async Task<TableResponseDto?> UpdateTableAsync(int id, UpdateTableDto tableDto)
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null) return null;

            if (tableDto.TableNumber.HasValue &&
                await _tableRepository.TableNumberExistsAsync(tableDto.TableNumber.Value))
                throw new InvalidOperationException($"Table number {tableDto.TableNumber.Value} already exists.");

            table.UpdateFromDto(tableDto);
            await _tableRepository.UpdateAsync(table);
            return table.ToResponseDto();
        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            return await _tableRepository.DeleteAsync(id);
        }

        /// <summary>
        /// KÄRNAN: Hitta lediga bord för bokning
        /// </summary>
        public async Task<IEnumerable<TableResponseDto>> GetAvailableTablesAsync(AvailableTablesQueryDto queryDto)
        {
            var tablesWithCapacity = await _tableRepository.GetTablesByCapacityAsync(queryDto.NumberOfGuests);
            var availableTables = new List<Models.Table>();

            foreach (var table in tablesWithCapacity)
            {
                var isAvailable = await _bookingRepository.IsTableAvailableAsync(table.Id, queryDto.BookingDateTime);
                if (isAvailable)
                    availableTables.Add(table);
            }

            return availableTables.Select(t => t.ToResponseDto());
        }

        public async Task<bool> IsTableNumberAvailableAsync(int tableNumber, int? excludeTableId = null)
        {
            return !await _tableRepository.TableNumberExistsAsync(tableNumber);
        }
    }
}
