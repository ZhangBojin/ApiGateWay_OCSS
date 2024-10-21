namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IRoleRepository
    {
        Task<bool> AddAsync(string role);
        Task DeleteAsync();
        Task UpdateAsync();
        Task SelectAllAsync();
        Task SaveChangesAsync();
    }
}
