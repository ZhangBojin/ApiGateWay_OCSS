namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IUserRoleRepository
    {
        Task AddAsync();
        Task DeleteAsync();
        Task UpdateAsync();
        Task SelectAllAsync();
        Task SaveChangesAsync();
    }
}
