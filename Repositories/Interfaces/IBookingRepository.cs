using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface IBookingRepository : IBaseRepository<Booking>
    {
        Task<Booking?> GetBookingWithDetailsAsync(int bookingId);
        Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date);
        Task<IEnumerable<Booking>> GetBookingsForTableAsync(int tableId, DateTime date);
        Task<bool> IsTableAvailableAsync(int tableId, DateTime bookingDateTime);
        Task<IEnumerable<Booking>> GetOverlappingBookingsAsync(int tableId, DateTime startTime, DateTime endTime);

        Task<IEnumerable<Booking>> GetUpcomingBookingsForCustomerAsync(int customerId);

        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
