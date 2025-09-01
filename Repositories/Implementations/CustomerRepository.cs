using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(RestaurantDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<Customer?> GetWithBookingsAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Bookings)
                    .ThenInclude(b => b.Table)
                .FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomersWithRecentBookingsAsync(DateTime fromDate)
        {
            return await _dbSet
                .Include(c => c.Bookings)
                .Where(c => c.Bookings.Any(b => b.BookingDateTime >= fromDate))
                .ToListAsync();
        }
    }
}
