using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class MenuInfoRepository(OCSS_DbContext context) : IMenuInfoRepository
    {
        private readonly OCSS_DbContext _context = context;

        public async Task<List<MenuInfo>> GetMenu(int roleId)
        {
            return await _context.MenuInfo.Where(m => m.RoleId == roleId).ToListAsync();
        }
    }
}
