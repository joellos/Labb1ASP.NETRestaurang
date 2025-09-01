using Labb1ASP.NETDatabas.DTOs.CustomerDTOs;
using Labb1ASP.NETDatabas.Extensions;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => c.ToResponseDto());
        }

        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer?.ToResponseDto();
        }

        public async Task<CustomerResponseDto?> GetCustomerWithBookingsAsync(int customerId)
        {
            var customer = await _customerRepository.GetWithBookingsAsync(customerId);
            return customer?.ToResponseDto();
        }

        public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto customerDto)
        {
            var existingCustomer = await _customerRepository.GetByPhoneNumberAsync(customerDto.PhoneNumber);
            if (existingCustomer != null)
                throw new InvalidOperationException("A customer with this phone number already exists.");

            var customer = customerDto.ToEntity();
            var created = await _customerRepository.CreateAsync(customer);
            return created.ToResponseDto();
        }

        public async Task<CustomerResponseDto?> UpdateCustomerAsync(int id, UpdateCustomerDto customerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return null;

            customer.UpdateFromDto(customerDto);
            await _customerRepository.UpdateAsync(customer);
            return customer.ToResponseDto();
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            return await _customerRepository.DeleteAsync(id);
        }

        public async Task<CustomerResponseDto> GetOrCreateCustomerAsync(string name, string phoneNumber, string email)
        {
            var existing = await _customerRepository.GetByPhoneNumberAsync(phoneNumber);
            if (existing != null)
                return existing.ToResponseDto();

            var customer = new Models.Customer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _customerRepository.CreateAsync(customer);
            return created.ToResponseDto();
        }

        public async Task<CustomerResponseDto?> FindCustomerAsync(string phoneNumber, string email)
        {
            var customer = await _customerRepository.GetByPhoneNumberAsync(phoneNumber);
            customer ??= await _customerRepository.GetByEmailAsync(email);
            return customer?.ToResponseDto();
        }
    }
}
