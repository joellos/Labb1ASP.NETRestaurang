using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Repositories.Interfaces
{
    public interface IAdministratorRepository : IBaseRepository<Administrator>
    {
        Task<Administrator?> GetByUsernameAsync(string username);>
        Task UpdateRefreshTokenAsync(int adminId, string refreshToken, DateTime expiryTime);
        Task UpdateLastLoginAsync(int adminId);
        Task<bool> UsernameExistsAsync(string username);
    }
}
