using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.Extensions;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Labb1ASP.NETDatabas.Services.Implementations
{
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly PasswordHasher<Administrator> _passwordHasher;

        public AdministratorService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
            _passwordHasher = new PasswordHasher<Administrator>();
        }

        public async Task<IEnumerable<AdministratorResponseDto>> GetAllAdministratorsAsync()
        {
            var administrators = await _administratorRepository.GetAllAsync();
            return administrators.Select(a => a.ToResponseDto());
        }

        public async Task<AdministratorResponseDto?> GetAdministratorByIdAsync(Guid id)
        {
            var administrator = await _administratorRepository.GetByIdAsync(id);
            return administrator?.ToResponseDto();
        }

        public async Task<AdministratorResponseDto> CreateAdministratorAsync(CreateAdministratorDto adminDto)
        {
            // Kontrollera om användarnamnet redan finns
            if (await _administratorRepository.UsernameExistsAsync(adminDto.Username))
                throw new InvalidOperationException($"Username '{adminDto.Username}' is already taken.");

            // Skapa administratör med hashad lösenord
            var administrator = new Administrator
            {
                Id = Guid.NewGuid(),
                Username = adminDto.Username,
                Name = adminDto.Name,
                Email = adminDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            // Hasha lösenordet
            administrator.PasswordHash = _passwordHasher.HashPassword(administrator, adminDto.Password);

            var created = await _administratorRepository.CreateAsync(administrator);
            return created.ToResponseDto();
        }

        public async Task<AdministratorResponseDto?> UpdateAdministratorAsync(Guid id, UpdateAdministratorDto adminDto)
        {
            var administrator = await _administratorRepository.GetByIdAsync(id);
            if (administrator == null)
                return null;

            // Kontrollera om nytt användarnamn är tillgängligt
            if (!string.IsNullOrEmpty(adminDto.Username) &&
                adminDto.Username != administrator.Username &&
                await _administratorRepository.UsernameExistsAsync(adminDto.Username))
            {
                throw new InvalidOperationException($"Username '{adminDto.Username}' is already taken.");
            }

            // Hasha nytt lösenord om det finns
            string? newPasswordHash = null;
            if (!string.IsNullOrEmpty(adminDto.Password))
            {
                newPasswordHash = _passwordHasher.HashPassword(administrator, adminDto.Password);
            }

            // Uppdatera administrator
            administrator.UpdateFromDto(adminDto, newPasswordHash);

            await _administratorRepository.UpdateAsync(administrator);
            return administrator.ToResponseDto();
        }

        public async Task<bool> DeleteAdministratorAsync(Guid id)
        {
            return await _administratorRepository.DeleteAsync(id);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeAdminId = null)
        {
            var existingAdmin = await _administratorRepository.GetByUsernameAsync(username);

            // Om ingen administratör med det användarnamnet finns, är det tillgängligt
            if (existingAdmin == null)
                return true;

            // Om vi exkluderar en specifik admin (för uppdateringar), kontrollera om det är samma admin
            if (excludeAdminId.HasValue && existingAdmin.Id == excludeAdminId.Value)
                return true;

            return false;
        }
    }
}
