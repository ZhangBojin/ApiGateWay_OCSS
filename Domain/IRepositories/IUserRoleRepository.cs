namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IUserRoleRepository
    {
        Task<bool> AddAsync(int userId,int roleId);
        Task DeleteAsync();
        Task UpdateAsync();
        Task SelectAllAsync();
        Task SaveChangesAsync();
    }
}
