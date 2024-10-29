using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class RoleRepository(OCSS_DbContext ocssDbContext) : IRoleRepository
    {
        private readonly OCSS_DbContext _ocssDbContext = ocssDbContext;

        public int GetRoleId(string name)
        {
            return _ocssDbContext.Roles.Where(r => r.RoleName == name)!.First().Id;
        }
    }
}
