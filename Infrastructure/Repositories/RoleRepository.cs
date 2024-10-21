using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class RoleRepository(OCSS_DbContext context) : IRoleRepository
    {
        private readonly OCSS_DbContext _context = context;


        public Task AddAsync()
        {
            throw new NotImplementedException();
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

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
