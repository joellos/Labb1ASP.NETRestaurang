using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface ITableRepository : IBaseRepository<Table>
    {
      
     
        Task<IEnumerable<Table>> GetActiveTablesAsync();

    
        Task<IEnumerable<Table>> GetTablesByCapacityAsync(int minCapacity);

     
        Task<bool> TableNumberExistsAsync(int tableNumber);

        Task<Table?> GetTableWithBookingsAsync(int tableId, DateTime date);
    }
}
