namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IRoleRepository
    {
        Task AddAsync();
        Task DeleteAsync();
        Task UpdateAsync();
        Task SelectAllAsync();
        Task SaveChangesAsync();
    }
}
