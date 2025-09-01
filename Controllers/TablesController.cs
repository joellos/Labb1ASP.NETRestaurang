using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableResponseDto>>> GetAllTables()
        {
            var tables = await _tableService.GetAllTablesAsync();
            return Ok(tables);
        }

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
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableResponseDto>> GetTableById(int id)
        {
            var table = await _tableService.GetTableByIdAsync(id);
            if (table == null)
                return NotFound();

            return Ok(table);
        }
    }
}
