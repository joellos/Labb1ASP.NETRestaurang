using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.DTOs.AuthDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<IEnumerable<AdministratorResponseDto>> GetAllAdministratorsAsync();
        Task<AdministratorResponseDto?> GetAdministratorByIdAsync(Guid id);
        Task<AdministratorResponseDto> CreateAdministratorAsync(CreateAdministratorDto adminDto);
        Task<AdministratorResponseDto?> UpdateAdministratorAsync(Guid id, UpdateAdministratorDto adminDto);
        Task<bool> DeleteAdministratorAsync(Guid id);
        Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeAdminId = null);
    }
}
