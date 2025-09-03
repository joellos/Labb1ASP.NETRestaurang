// Controllers/TablesController.cs - ADMIN-ONLY med full CRUD

using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")] // HELA CONTROLLERN KRÄVER ADMIN
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        // GET: api/tables - Admin ser alla bord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableResponseDto>>> GetAllTables()
        {
            try
            {
                var tables = await _tableService.GetAllTablesAsync();
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/tables/active - Admin ser bara aktiva bord
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TableResponseDto>>> GetActiveTables()
        {
            try
            {
                var tables = await _tableService.GetActiveTablesAsync();
                return Ok(tables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/tables/{id} - Admin ser specifikt bord
        [HttpGet("{id}")]
        public async Task<ActionResult<TableResponseDto>> GetTableById(int id)
        {
            try
            {
                var table = await _tableService.GetTableByIdAsync(id);
                if (table == null)
                    return NotFound($"Table with ID {id} not found.");

                return Ok(table);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/tables - Skapa nytt bord
        [HttpPost]
        public async Task<ActionResult<TableResponseDto>> CreateTable([FromBody] CreateTableDto tableDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var table = await _tableService.CreateTableAsync(tableDto);
                return CreatedAtAction(nameof(GetTableById), new { id = table.Id }, table);
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

        // PUT: api/tables/{id} - Uppdatera bord
        [HttpPut("{id}")]
        public async Task<ActionResult<TableResponseDto>> UpdateTable(int id, [FromBody] UpdateTableDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var table = await _tableService.UpdateTableAsync(id, updateDto);
                if (table == null)
                    return NotFound($"Table with ID {id} not found.");

                return Ok(table);
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

        // DELETE: api/tables/{id} - Ta bort bord
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            try
            {
                var result = await _tableService.DeleteTableAsync(id);
                if (!result)
                    return NotFound($"Table with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/tables/{id}/toggle-active - Aktivera/inaktivera bord
        [HttpPut("{id}/toggle-active")]
        public async Task<ActionResult<TableResponseDto>> ToggleTableActive(int id)
        {
            try
            {
                var table = await _tableService.GetTableByIdAsync(id);
                if (table == null)
                    return NotFound($"Table with ID {id} not found.");

                var updateDto = new UpdateTableDto { IsActive = !table.IsActive };
                var updatedTable = await _tableService.UpdateTableAsync(id, updateDto);

                return Ok(updatedTable);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}