using Labb1ASP.NETDatabas.Data;
using Labb1ASP.NETDatabas.Models;
using Labb1ASP.NETDatabas.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Labb1ASP.NETDatabas.Repositories.Implementations
{
    public class AdministratorRepository : IAdministratorRepository
    {
        protected readonly RestaurantDbContext _context;
        protected readonly DbSet<Administrator> _dbSet;

        public AdministratorRepository(RestaurantDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Administrator>();
        }

        public async Task<Administrator?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Administrator>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Administrator> CreateAsync(Administrator entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Administrator> UpdateAsync(Administrator entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public async Task<Administrator?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Username.ToLower() == username.ToLower());
        }

        public async Task UpdateRefreshTokenAsync(Guid adminId, string refreshToken, DateTime expiryTime)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin != null)
            {
                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiryTime = expiryTime;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLastLoginAsync(Guid adminId)
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
