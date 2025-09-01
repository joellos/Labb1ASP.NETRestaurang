using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class AdministratorRepository : BaseRepository<Administrator>, IAdministratorRepository
    {
        public AdministratorRepository(RestaurantDbContext context) : base(context)
        {
        }

        public async Task<Administrator?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Username.ToLower() == username.ToLower());
        }

        public async Task UpdateRefreshTokenAsync(int adminId, string refreshToken, DateTime expiryTime)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin != null)
            {
                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiryTime = expiryTime;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLastLoginAsync(int adminId)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin != null)
            {
                admin.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet
                .AnyAsync(a => a.Username.ToLower() == username.ToLower());
        }
    }
}
