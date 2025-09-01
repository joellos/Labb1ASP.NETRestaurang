using Labb1ASP.NETDatabas.DTOs.BookingDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto?> GetBookingWithDetailsAsync(int id);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByDateAsync(DateTime date);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<BookingResponseDto>> GetUpcomingBookingsForCustomerAsync(int customerId);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<BookingResponseDto?> UpdateBookingAsync(int id, UpdateBookingDto updateDto);

        Task<bool> DeleteBookingAsync(int id);
        Task<(bool IsValid, string ErrorMessage)> ValidateBookingAsync(int tableId, DateTime bookingDateTime, int numberOfGuests);
    }
}
