using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class TableRepository : BaseRepository<Table>, ITableRepository
    {
        public TableRepository(RestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Table>> GetActiveTablesAsync()
        {
            return await _dbSet
                .Where(t => t.IsActive)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Table>> GetTablesByCapacityAsync(int minCapacity)
        {
            return await _dbSet
                .Where(t => t.IsActive && t.Capacity >= minCapacity)
                .OrderBy(t => t.Capacity)
                .ThenBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<bool> TableNumberExistsAsync(int tableNumber)
        {
            return await _dbSet
                .AnyAsync(t => t.TableNumber == tableNumber);
        }

        public async Task<Table?> GetTableWithBookingsAsync(int tableId, DateTime date)
        {
            return await _dbSet
                .Include(t => t.Bookings.Where(b => b.BookingDateTime.Date == date.Date))
                .FirstOrDefaultAsync(t => t.Id == tableId);
        }
    }
}
