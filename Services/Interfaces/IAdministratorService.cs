using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.DTOs.AuthDTOs;

namespace Labb1ASP.NETDatabas.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<IEnumerable<AdministratorResponseDto>> GetAllAdministratorsAsync();
        Task<AdministratorResponseDto?> GetAdministratorByIdAsync(int id);
        Task<AdministratorResponseDto> CreateAdministratorAsync(CreateAdministratorDto adminDto);
        Task<AdministratorResponseDto?> UpdateAdministratorAsync(int id, UpdateAdministratorDto adminDto);
        Task<bool> DeleteAdministratorAsync(int id);
        Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto);
        Task UpdateRefreshTokenAsync(int adminId, string refreshToken, DateTime expiryTime);
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> IsUsernameAvailableAsync(string username, int? excludeAdminId = null);
    }
}
