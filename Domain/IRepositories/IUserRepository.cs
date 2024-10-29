using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Infrastructure.Repositories;

namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<bool> AddAsync(Users user);
        Task<UserInfo?> GetByEmail(string email);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
        Task SaveChangesAsync();

        string GenerateToken(UserInfo users, string role);
    }
}
