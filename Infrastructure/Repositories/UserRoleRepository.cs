using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories;

public class UserRoleRepository(OCSS_DbContext context) : IUserRoleRepository
{
    private readonly OCSS_DbContext _context = context;


    public async Task<bool> AddAsync(int userId, int roleId)
    {
        if (await _context.UserRoles.AnyAsync(u => u.UserId == userId && u.RoleId == roleId)) return false;
        await _context.UserRoles.AddAsync(new UserRole()
        {
            UserId = userId,
            RoleId = roleId,
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