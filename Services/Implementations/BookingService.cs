using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.Extensions;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITableRepository _tableRepository;
        private readonly ICustomerRepository _customerRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ITableRepository tableRepository,
            ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _tableRepository = tableRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(b => b.ToResponseDto());
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            return booking?.ToResponseDto();
        }

        public async Task<BookingResponseDto?> GetBookingWithDetailsAsync(int id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            return booking?.ToResponseDto();
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByDateAsync(DateTime date)
        {
            var bookings = await _bookingRepository.GetBookingsByDateAsync(date);
            return bookings.Select(b => b.ToResponseDto());
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _bookingRepository.GetBookingsByDateRangeAsync(startDate, endDate);
            return bookings.Select(b => b.ToResponseDto());
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUpcomingBookingsForCustomerAsync(int customerId)
        {
            var bookings = await _bookingRepository.GetUpcomingBookingsForCustomerAsync(customerId);
            return bookings.Select(b => b.ToResponseDto());
        }

        /// <summary>
        /// KÄRNAN I VÅRT BOKNINGSSYSTEM
        /// Denna metod innehåller ALL affärslogik för att skapa bokningar
        /// </summary>
        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            // 1. VALIDERA BOKNINGEN
            var validationResult = await ValidateBookingAsync(
                bookingDto.TableId,
                bookingDto.BookingDateTime,
                bookingDto.NumberOfGuests);

            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(validationResult.ErrorMessage);
            }

            // 2. HITTA ELLER SKAPA KUND (förhindrar dubletter)
            var customer = await GetOrCreateCustomerAsync(
                bookingDto.CustomerName,
                bookingDto.PhoneNumber,
                bookingDto.Email);

            // 3. SKAPA BOKNING
            var booking = new Booking
            {
                CustomerId = customer.Id,
                TableId = bookingDto.TableId,
                BookingDateTime = bookingDto.BookingDateTime,
                NumberOfGuests = bookingDto.NumberOfGuests,
                SpecialRequests = bookingDto.SpecialRequests,
                CreatedAt = DateTime.UtcNow
            };

            // 4. SPARA I DATABAS
            var createdBooking = await _bookingRepository.CreateAsync(booking);

            // 5. HÄMTA MED FULLSTÄNDIG DATA OCH RETURNERA DTO
            var bookingWithDetails = await _bookingRepository.GetBookingWithDetailsAsync(createdBooking.Id);
            return bookingWithDetails!.ToResponseDto();
        }

        public async Task<BookingResponseDto?> UpdateBookingAsync(int id, UpdateBookingDto updateDto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return null;

            // Om datum eller antal gäster ändras, validera igen
            if (updateDto.BookingDateTime.HasValue || updateDto.NumberOfGuests.HasValue)
            {
                var newDateTime = updateDto.BookingDateTime ?? booking.BookingDateTime;
                var newGuests = updateDto.NumberOfGuests ?? booking.NumberOfGuests;

                // Exkludera denna bokning från tillgänglighetskontrollen
                var overlappingBookings = await _bookingRepository.GetOverlappingBookingsAsync(
                    booking.TableId, newDateTime, newDateTime.AddHours(2));

                if (overlappingBookings.Any(b => b.Id != id))
                {
                    throw new InvalidOperationException("Table is not available at the requested time.");
                }

                // Kontrollera bordets kapacitet
                var table = await _tableRepository.GetByIdAsync(booking.TableId);
                if (table != null && newGuests > table.Capacity)
                {
                    throw new InvalidOperationException($"Table capacity is {table.Capacity}, but {newGuests} guests requested.");
                }
            }

            // Uppdatera booking
            booking.UpdateFromDto(updateDto);
            await _bookingRepository.UpdateAsync(booking);

            // Returnera uppdaterad data
            var updatedBooking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            return updatedBooking?.ToResponseDto();
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            return await _bookingRepository.DeleteAsync(id);
        }

        /// <summary>
        /// AFFÄRSLOGIK: Validera om bokning är möjlig
        /// Kontrollerar: Bord finns, är aktivt, har kapacitet, och är ledigt
        /// </summary>
        public async Task<(bool IsValid, string ErrorMessage)> ValidateBookingAsync(int tableId, DateTime bookingDateTime, int numberOfGuests)
        {
            // 1. Kontrollera att bordet finns och är aktivt
            var table = await _tableRepository.GetByIdAsync(tableId);
            if (table == null)
                return (false, "Table not found.");

            if (!table.IsActive)
                return (false, "Table is not active.");

            // 2. Kontrollera kapacitet
            if (numberOfGuests > table.Capacity)
                return (false, $"Table capacity is {table.Capacity}, but {numberOfGuests} guests requested.");

            // 3. Kontrollera att bokningen är i framtiden
            if (bookingDateTime <= DateTime.UtcNow)
                return (false, "Booking must be in the future.");

            // 4. Kontrollera att restaurangen är öppen (exempel: 10:00-23:00)
            var bookingTime = bookingDateTime.TimeOfDay;
            if (bookingTime < TimeSpan.FromHours(10) || bookingTime > TimeSpan.FromHours(21)) // Sista bokning 21:00 (slutar 23:00)
                return (false, "Restaurant is open between 10:00 and 21:00 for bookings.");

            // 5. Kontrollera tillgänglighet (2-timmars regel)
            var isAvailable = await _bookingRepository.IsTableAvailableAsync(tableId, bookingDateTime);
            if (!isAvailable)
                return (false, "Table is not available at the requested time. Each booking reserves the table for 2 hours.");

            return (true, string.Empty);
        }

        /// <summary>
        /// HJÄLPMETOD: Hitta befintlig kund eller skapa ny
        /// Förhindrar dubletter i systemet
        /// </summary>
        private async Task<Customer> GetOrCreateCustomerAsync(string name, string phoneNumber, string email)
        {
            // Försök hitta befintlig kund via telefon
            var existingCustomer = await _customerRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existingCustomer != null)
            {
                // Uppdatera eventuell information (om namn eller email ändrats)
                var updated = false;
                if (existingCustomer.Name != name)
                {
                    existingCustomer.Name = name;
                    updated = true;
                }
                if (existingCustomer.Email != email)
                {
                    existingCustomer.Email = email;
                    updated = true;
                }

                if (updated)
                    await _customerRepository.UpdateAsync(existingCustomer);

                return existingCustomer;
            }

            // Skapa ny kund
            var newCustomer = new Customer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            return await _customerRepository.CreateAsync(newCustomer);
        }
    }
}
