using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class UserRepository(OCSS_DbContext context) : IUserRepository
    {
        private readonly OCSS_DbContext _context = context;

         
        public async Task<bool> AddAsync(Users user)
        {
            if(await _context.Users.FirstOrDefaultAsync(u => u!.Email == user.Email) != null) return false;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreationTime=DateTime.Now;
            await _context.Users.AddAsync(user);
            return true;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u!.Email == email);
            return user != null && VerifyPassword(password, user.Password);
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

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
