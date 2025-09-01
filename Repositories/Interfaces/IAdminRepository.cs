using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface IAdministratorRepository
    {
        // Guid-baserade metoder för Administrator
        Task<Administrator?> GetByIdAsync(Guid id);
        Task<IEnumerable<Administrator>> GetAllAsync();
        Task<Administrator> CreateAsync(Administrator entity);
        Task<Administrator> UpdateAsync(Administrator entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        // Administrator-specifika metoder
        Task<Administrator?> GetByUsernameAsync(string username);
        Task UpdateRefreshTokenAsync(Guid adminId, string refreshToken, DateTime expiryTime);
        Task UpdateLastLoginAsync(Guid adminId);
        Task<bool> UsernameExistsAsync(string username);
    }
}
