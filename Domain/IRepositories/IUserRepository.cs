using ApiGateWay_OCSS.Domain.Entities;

namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(Users user);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task DeleteAsync();
        Task UpdateAsync();
        Task SelectAllAsync();
        Task SaveChangesAsync();
    }
}
