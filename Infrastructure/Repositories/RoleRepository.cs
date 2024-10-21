using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class RoleRepository(OCSS_DbContext context) : IRoleRepository
    {
        private readonly OCSS_DbContext _context = context;


        public async Task<bool> AddAsync(string role)
        {
            if (await _context.Roles.FirstOrDefaultAsync(u => u!.RoleName == role) != null) return false;
            await _context.AddAsync(new Roles()
            {
                RoleName = role,
                CreationTime = DateTime.Now
            });
            return true;
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync()
        {
            throw new NotImplementedException();
        }

        public Task SelectAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
