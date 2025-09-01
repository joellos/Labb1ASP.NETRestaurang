using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(RestaurantDbContext context) : base(context)
        {
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int bookingId)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Include(b => b.Table)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Include(b => b.Table)
                .Where(b => b.BookingDateTime.Date == date.Date)
                .OrderBy(b => b.BookingDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsForTableAsync(int tableId, DateTime date)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Where(b => b.TableId == tableId && b.BookingDateTime.Date == date.Date)
                .OrderBy(b => b.BookingDateTime)
                .ToListAsync();
        }

        /// <summary>
        /// KÄRNAN I VÅRT BOKNINGSSYSTEM
        /// Kontrollerar om bord är ledigt vid specifik tid
        /// Affärsregel: En bokning blockerar bordet i 2 timmar
        /// </summary>
        public async Task<bool> IsTableAvailableAsync(int tableId, DateTime bookingDateTime)
        {
            // Beräkna slutdatum för den nya bokningen (2 timmar)
            var newBookingEndTime = bookingDateTime.AddHours(2);

            // Hitta alla bokningar för detta bord på samma dag
            var existingBookings = await _dbSet
                .Where(b => b.TableId == tableId && b.BookingDateTime.Date == bookingDateTime.Date)
                .Select(b => new { b.BookingDateTime, EndTime = b.BookingDateTime.AddHours(2) })
                .ToListAsync();

            // Kontrollera överlappning med befintliga bokningar
            foreach (var existing in existingBookings)
            {
                // Överlappning finns om:
                // - Ny bokning startar innan befintlig slutar OCH
                // - Ny bokning slutar efter befintlig startar
                if (bookingDateTime < existing.EndTime && newBookingEndTime > existing.BookingDateTime)
                {
                    return false; // Överlappning - bord ej ledigt
                }
            }

            return true; // Ingen överlappning - bord ledigt
        }

        public async Task<IEnumerable<Booking>> GetOverlappingBookingsAsync(int tableId, DateTime startTime, DateTime endTime)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Where(b => b.TableId == tableId)
                .Where(b => b.BookingDateTime < endTime && b.BookingDateTime.AddHours(2) > startTime)
                .OrderBy(b => b.BookingDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsForCustomerAsync(int customerId)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(b => b.Table)
                .Where(b => b.CustomerId == customerId && b.BookingDateTime > now)
                .OrderBy(b => b.BookingDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(b => b.Customer)
                .Include(b => b.Table)
                .Where(b => b.BookingDateTime >= startDate && b.BookingDateTime <= endDate)
                .OrderBy(b => b.BookingDateTime)
                .ToListAsync();
        }
    }
}
