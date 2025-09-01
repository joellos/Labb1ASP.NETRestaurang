using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetWithBookingsAsync(int customerId);
        Task<IEnumerable<Customer>> GetCustomersWithRecentBookingsAsync(DateTime fromDate);
    }
}
