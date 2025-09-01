using Labb1ASP.NETDatabas.DTOs.CustomerDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();
        Task<CustomerResponseDto?> GetCustomerByIdAsync(int id);
        Task<CustomerResponseDto?> GetCustomerWithBookingsAsync(int customerId);
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto customerDto);
        Task<CustomerResponseDto?> UpdateCustomerAsync(int id, UpdateCustomerDto customerDto);
        Task<bool> DeleteCustomerAsync(int id);
        Task<CustomerResponseDto> GetOrCreateCustomerAsync(string name, string phoneNumber, string email);
        Task<CustomerResponseDto?> FindCustomerAsync(string phoneNumber, string email);
    }
}
