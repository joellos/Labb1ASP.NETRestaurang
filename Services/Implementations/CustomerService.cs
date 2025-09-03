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
            // Kontrollera om EMAIL redan finns (email är nu primär identifierare)
            var existingCustomer = await _customerRepository.GetByEmailAsync(customerDto.Email);
            if (existingCustomer != null)
                throw new InvalidOperationException($"A customer with email '{customerDto.Email}' already exists.");

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
            // EMAIL som primär identifierare
            var existing = await _customerRepository.GetByEmailAsync(email);
            if (existing != null)
            {
                // Uppdatera telefon och namn automatiskt
                var updated = false;
                if (existing.Name != name)
                {
                    existing.Name = name;
                    updated = true;
                }
                if (existing.PhoneNumber != phoneNumber)
                {
                    existing.PhoneNumber = phoneNumber;
                    updated = true;
                }

                if (updated)
                    await _customerRepository.UpdateAsync(existing);

                return existing.ToResponseDto();
            }

            // Skapa ny kund
            var newCustomer = new Models.Customer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _customerRepository.CreateAsync(newCustomer);
            return created.ToResponseDto();
        }

 
        public async Task<CustomerResponseDto?> FindCustomerAsync(string phoneNumber, string email)
        {
            Models.Customer? customer = null;

            // SÖK FÖRST PÅ EMAIL (primär identifierare)
            if (!string.IsNullOrEmpty(email))
            {
                customer = await _customerRepository.GetByEmailAsync(email);
            }

            // Sök på telefon bara om email inte gav resultat
            if (customer == null && !string.IsNullOrEmpty(phoneNumber))
            {
                customer = await _customerRepository.GetByPhoneNumberAsync(phoneNumber);
            }

            return customer?.ToResponseDto();
        }

        public async Task<CustomerResponseDto?> FindCustomerByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            return customer?.ToResponseDto();
        }

        public async Task<IEnumerable<CustomerResponseDto>> FindPotentialDuplicatesAsync()
        {
            var allCustomers = await _customerRepository.GetAllAsync();

            // Hitta kunder med samma email (ska inte finnas med nya systemet)
            var emailDuplicates = allCustomers
                .GroupBy(c => c.Email.ToLower())
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            return emailDuplicates.Select(c => c.ToResponseDto());
        }
    }
}