using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.CustomerDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")] // ENDAST ADMINS
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;

        public CustomerController(ICustomerService customerService, IBookingService bookingService)
        {
            _customerService = customerService;
            _bookingService = bookingService;
        }

        /// <summary>
        /// Admin ser alla kunder
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin ser specifik kund
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound($"Customer with ID {id} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin ser kund med alla bokningar
        /// </summary>
        [HttpGet("{id}/with-bookings")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerWithBookings(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerWithBookingsAsync(id);
                if (customer == null)
                    return NotFound($"Customer with ID {id} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin söker kund via EMAIL (primär metod)
        /// </summary>
        [HttpGet("search/email/{email}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerByEmail(string email)
        {
            try
            {
                var customer = await _customerService.FindCustomerByEmailAsync(email);
                if (customer == null)
                    return NotFound($"Customer with email {email} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin söker kund via telefon (backup metod)
        /// </summary>
        [HttpGet("search/phone/{phoneNumber}")]
        public async Task<ActionResult<CustomerResponseDto>> GetCustomerByPhone(string phoneNumber)
        {
            try
            {
                var customer = await _customerService.FindCustomerAsync(phoneNumber, "");
                if (customer == null)
                    return NotFound($"Customer with phone number {phoneNumber} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin skapar kund manuellt
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerResponseDto>> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var customer = await _customerService.CreateCustomerAsync(customerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
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
        /// Admin uppdaterar kund
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var customer = await _customerService.UpdateCustomerAsync(id, updateDto);
                if (customer == null)
                    return NotFound($"Customer with ID {id} not found.");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin tar bort kund
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                    return NotFound($"Customer with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin ser alla kundens bokningar
        /// </summary>
        [HttpGet("{customerId}/bookings")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetCustomerBookings(int customerId)
        {
            try
            {
                var bookings = await _bookingService.GetUpcomingBookingsForCustomerAsync(customerId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin hittar potentiella dubletter (med nya systemet bör det vara mycket få)
        /// </summary>
        [HttpGet("duplicates")]
        public async Task<ActionResult<object>> FindPotentialDuplicates()
        {
            try
            {
                var allCustomers = await _customerService.GetAllCustomersAsync();

                // Gruppera på email för att hitta dubletter (bör inte finnas med nya systemet)
                var emailDuplicates = allCustomers
                    .GroupBy(c => c.Email.ToLower())
                    .Where(g => g.Count() > 1)
                    .Select(g => new {
                        Email = g.Key,
                        Count = g.Count(),
                        Customers = g.ToList()
                    });

                // Gruppera på liknande namn för manuell granskning
                var nameDuplicates = allCustomers
                    .GroupBy(c => c.Name.ToLower().Replace(" ", ""))
                    .Where(g => g.Count() > 1)
                    .Select(g => new {
                        Name = g.First().Name,
                        Count = g.Count(),
                        Customers = g.ToList()
                    });

                return Ok(new
                {
                    EmailDuplicates = emailDuplicates,
                    NameDuplicates = nameDuplicates,
                    Message = emailDuplicates.Any() ? "Email duplicates found - this shouldn't happen with the new system!" : "No email duplicates found (good!)"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin ser kundstatistik
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetCustomerStats()
        {
            try
            {
                var allCustomers = await _customerService.GetAllCustomersAsync();

                var stats = new
                {
                    TotalCustomers = allCustomers.Count(),
                    CustomersWithBookings = allCustomers.Count(c => c.RecentBookings.Any()),
                    CustomersThisMonth = allCustomers.Count(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-30)),
                    CustomersThisWeek = allCustomers.Count(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
                    TopCustomersByBookings = allCustomers
                        .Where(c => c.RecentBookings.Any())
                        .OrderByDescending(c => c.RecentBookings.Count)
                        .Take(10)
                        .Select(c => new {
                            c.Name,
                            c.Email,
                            c.PhoneNumber,
                            BookingCount = c.RecentBookings.Count,
                            LastBooking = c.RecentBookings.OrderByDescending(b => b.BookingDateTime).FirstOrDefault()?.BookingDateTime
                        }),
                    RecentCustomers = allCustomers
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(5)
                        .Select(c => new {
                            c.Id,
                            c.Name,
                            c.Email,
                            c.CreatedAt,
                            BookingCount = c.RecentBookings.Count
                        })
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin söker kunder med flexibel sökning (namn, email, telefon)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> SearchCustomers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty.");

                var allCustomers = await _customerService.GetAllCustomersAsync();
                var searchLower = searchTerm.ToLower();

                var matchingCustomers = allCustomers.Where(c =>
                    c.Name.ToLower().Contains(searchLower) ||
                    c.Email.ToLower().Contains(searchLower) ||
                    c.PhoneNumber.Contains(searchTerm)
                ).ToList();

                return Ok(matchingCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}