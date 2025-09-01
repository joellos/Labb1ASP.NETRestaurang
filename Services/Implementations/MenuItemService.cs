using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.Extensions;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();
            return menuItems.Select(m => m.ToResponseDto());
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetAvailableMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.GetAvailableItemsAsync();
            return menuItems.Select(m => m.ToResponseDto());
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetPopularMenuItemsAsync()
        {
            var menuItems = await _menuItemRepository.GetPopularItemsAsync();
            return menuItems.Select(m => m.ToResponseDto());
        }

        public async Task<IEnumerable<MenuItemResponseDto>> GetMenuItemsByCategoryAsync(string category)
        {
            var menuItems = await _menuItemRepository.GetItemsByCategoryAsync(category);
            return menuItems.Select(m => m.ToResponseDto());
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _menuItemRepository.GetCategoriesAsync();
        }

        public async Task<MenuItemResponseDto?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            return menuItem?.ToResponseDto();
        }

        public async Task<MenuItemResponseDto> CreateMenuItemAsync(CreateMenuItemDto menuItemDto)
        {
            var menuItem = menuItemDto.ToEntity();
            var created = await _menuItemRepository.CreateAsync(menuItem);
            return created.ToResponseDto();
        }

        public async Task<MenuItemResponseDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto menuItemDto)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return null;

            menuItem.UpdateFromDto(menuItemDto);
            await _menuItemRepository.UpdateAsync(menuItem);
            return menuItem.ToResponseDto();
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            return await _menuItemRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MenuItemResponseDto>> SearchMenuItemsAsync(string searchTerm)
        {
            var menuItems = await _menuItemRepository.SearchMenuItemsAsync(searchTerm);
            return menuItems.Select(m => m.ToResponseDto());
        }

        public async Task<bool> ToggleAvailabilityAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return false;

            menuItem.IsAvailable = !menuItem.IsAvailable;
            await _menuItemRepository.UpdateAsync(menuItem);
            return true;
        }

        public async Task<bool> TogglePopularAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                return false;

            menuItem.IsPopular = !menuItem.IsPopular;
            await _menuItemRepository.UpdateAsync(menuItem);
            return true;
        }
    }

}
