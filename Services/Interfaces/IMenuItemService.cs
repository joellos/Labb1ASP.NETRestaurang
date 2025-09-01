using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync();
        Task<IEnumerable<MenuItemResponseDto>> GetAvailableMenuItemsAsync();
        Task<IEnumerable<MenuItemResponseDto>> GetPopularMenuItemsAsync();
        Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<MenuItemResponseDto?> GetMenuItemByIdAsync(int id);
        Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemDto menuItemDto);
        Task<MenuItemResponseDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto menuItemDto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm);
        Task<bool> ToggleAvailabilityAsync(int id);
        Task<bool> TogglePopularAsync(int id);
    }
}
