using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ITableService _tableService;
        private readonly ICustomerService _customerService;

        public BookingController(IBookingService bookingService, ITableService tableService, ICustomerService customerService)
        {
            _bookingService = bookingService;
            _tableService = tableService;
            _customerService = customerService;
        }

        // PUBLIKA ENDPOINTS - För kunder

        /// <summary>
        /// Kunder söker lediga bord
        /// </summary>
        [HttpGet("available-tables")]
        [AllowAnonymous]
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

        /// <summary>
        /// Kunder gör bokningar
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
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

        /// <summary>
        /// PRIMÄR: Kunder kan hitta sina bokningar via EMAIL
        /// </summary>
        [HttpGet("customer/email/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByEmail(string email)
        {
            try
            {
                // Hitta kund via email (primär identifierare)
                var customer = await _customerService.FindCustomerByEmailAsync(email);
                if (customer == null)
                    return NotFound("No bookings found for this email address.");

                // Hämta kundens kommande bokningar
                var bookings = await _bookingService.GetUpcomingBookingsForCustomerAsync(customer.Id);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// BACKUP: Kunder kan hitta sina bokningar via TELEFON (fallback)
        /// </summary>
        [HttpGet("customer/phone/{phoneNumber}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByPhoneNumber(string phoneNumber)
        {
            try
            {
                var customer = await _customerService.FindCustomerAsync(phoneNumber, "");
                if (customer == null)
                    return NotFound("No bookings found for this phone number.");

                var bookings = await _bookingService.GetUpcomingBookingsForCustomerAsync(customer.Id);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // ADMIN ENDPOINTS - Kräver inloggning

        /// <summary>
        /// Admin ser alla bokningar
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
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

        /// <summary>
        /// Admin ser specifik bokning
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
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

        /// <summary>
        /// Admin uppdaterar bokning
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
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

        /// <summary>
        /// Admin tar bort bokning (avbokning)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
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

        /// <summary>
        /// Admin ser bokningar per datum
        /// </summary>
        [HttpGet("by-date")]
        [Authorize(Policy = "AdminPolicy")]
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

        /// <summary>
        /// Admin ser bokningar för datumspan
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}