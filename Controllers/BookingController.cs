using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ITableService _tableService;

        public BookingController(IBookingService bookingService, ITableService tableService)
        {
            _bookingService = bookingService;
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("available-tables")]
        public async Task<ActionResult<IEnumerable<TableResponseDto>>> GetAvailableTables([FromQuery] AvailableTablesQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var availableTables = await _tableService.GetAvailableTablesAsync(query);
                return Ok(availableTables);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var booking = await _bookingService.CreateBookingAsync(bookingDto);
                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBookingById(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingWithDetailsAsync(id);
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found.");

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponseDto>> UpdateBooking(int id, [FromBody] UpdateBookingDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var booking = await _bookingService.UpdateBookingAsync(id, updateDto);
                if (booking == null)
                    return NotFound($"Booking with ID {id} not found.");

                return Ok(booking);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var result = await _bookingService.DeleteBookingAsync(id);
                if (!result)
                    return NotFound($"Booking with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByDate([FromQuery] DateTime date)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateAsync(date);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
