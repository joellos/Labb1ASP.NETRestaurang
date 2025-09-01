using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface IMenuItemRepository : IBaseRepository<MenuItem>
    {
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();
        Task<IEnumerable<MenuItem>> GetPopularItemsAsync();
        Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm);
    }
}
