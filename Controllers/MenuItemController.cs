using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // PUBLIKA ENDPOINTS - För kunder att se menyn

        // GET: api/menuitem - Alla tillgängliga menyrätter (för kunder)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetAvailableMenuItems()
        {
            try
            {
                var menuItems = await _menuItemService.GetAvailableMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/menuitem/popular - Populära rätter (för kunder)
        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetPopularMenuItems()
        {
            try
            {
                var menuItems = await _menuItemService.GetPopularMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/menuitem/categories - Alla kategorier (för kunder)
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _menuItemService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/menuitem/category/{category} - Rätter per kategori (för kunder)
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetMenuItemsByCategory(string category)
        {
            try
            {
                var menuItems = await _menuItemService.GetMenuItemsByCategoryAsync(category);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/menuitem/search?term=pasta - Sök i menyn (för kunder)
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> SearchMenuItems([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest("Search term cannot be empty.");

                var menuItems = await _menuItemService.SearchMenuItemsAsync(term);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ADMIN ENDPOINTS - Kräver inloggning

        // GET: api/menuitem/admin/all - Admin ser ALLA rätter (även inaktiva)
        [HttpGet("admin/all")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<IEnumerable<MenuItemResponseDto>>> GetAllMenuItems()
        {
            try
            {
                var menuItems = await _menuItemService.GetAllMenuItemsAsync();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/menuitem/{id} - Hämta specifik rätt
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<MenuItemResponseDto>> GetMenuItemById(int id)
        {
            try
            {
                var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                if (menuItem == null)
                    return NotFound($"Menu item with ID {id} not found.");

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/menuitem - Skapa ny rätt
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<MenuItemResponseDto>> CreateMenuItem([FromBody] CreateMenuItemDto menuItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var menuItem = await _menuItemService.CreateMenuItemAsync(menuItemDto);
                return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/menuitem/{id} - Uppdatera rätt
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<MenuItemResponseDto>> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var menuItem = await _menuItemService.UpdateMenuItemAsync(id, updateDto);
                if (menuItem == null)
                    return NotFound($"Menu item with ID {id} not found.");

                return Ok(menuItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/menuitem/{id} - Ta bort rätt
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            try
            {
                var result = await _menuItemService.DeleteMenuItemAsync(id);
                if (!result)
                    return NotFound($"Menu item with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/menuitem/{id}/toggle-availability - Visa/dölj rätt från menyn
        [HttpPut("{id}/toggle-availability")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<MenuItemResponseDto>> ToggleAvailability(int id)
        {
            try
            {
                var result = await _menuItemService.ToggleAvailabilityAsync(id);
                if (!result)
                    return NotFound($"Menu item with ID {id} not found.");

                var updatedMenuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                return Ok(updatedMenuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/menuitem/{id}/toggle-popular - Markera/avmarkera som populär
        [HttpPut("{id}/toggle-popular")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<MenuItemResponseDto>> TogglePopular(int id)
        {
            try
            {
                var result = await _menuItemService.TogglePopularAsync(id);
                if (!result)
                    return NotFound($"Menu item with ID {id} not found.");

                var updatedMenuItem = await _menuItemService.GetMenuItemByIdAsync(id);
                return Ok(updatedMenuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}