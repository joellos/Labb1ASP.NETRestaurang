using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(RestaurantDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            return await _dbSet
                .Where(m => m.IsAvailable)
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetPopularItemsAsync()
        {
            return await _dbSet
                .Where(m => m.IsPopular && m.IsAvailable)
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(m => m.Category.ToLower() == category.ToLower() && m.IsAvailable)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _dbSet
                .Where(m => m.IsAvailable)
                .Select(m => m.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchMenuItemsAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _dbSet
                .Where(m => m.IsAvailable &&
                           (m.Name.ToLower().Contains(lowerSearchTerm) ||
                            m.Description.ToLower().Contains(lowerSearchTerm)))
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }
    }
}
