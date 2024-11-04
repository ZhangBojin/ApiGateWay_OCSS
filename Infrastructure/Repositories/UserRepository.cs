using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateWay_OCSS.Infrastructure.Repositories
{
    public class UserRepository(OCSS_DbContext context,IConfiguration configuration) : IUserRepository
    {
        private readonly OCSS_DbContext _context = context;
        private readonly IConfiguration _configuration= configuration;

         
        public async Task<bool> AddAsync(Users user)
        {
            if(await _context.Users.AnyAsync(u => u!.Email == user.Email)) return false;
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreationTime=DateTime.Now;
            await _context.Users.AddAsync(user);
            return true;
        }

        public async Task<UserInfo?> GetByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u!.Email == email);
            if(user == null) return null;
            return await _context.UserInfo.FirstAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user =  await _context.Users.FirstOrDefaultAsync(u => u!.Email == email);
            return user != null && VerifyPassword(password, user.Password);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public string GenerateToken(UserInfo users,string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings")["SecretKey"]!));
            //定义如何对jwt进行签名
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration.GetSection("JwtSettings")["Issuer"];
            var audience = _configuration.GetSection("JwtSettings")["Audience"];
            var expiryInMinutes = int.Parse(_configuration.GetSection("JwtSettings")["TokenExpiryInMinutes"]!);
            var claims = new[]
            {
                //ClaimTypes.NameIdentifier 用于表示用户的唯一标识符
                new Claim(ClaimTypes.NameIdentifier,users.Id.ToString()),
                new Claim(ClaimTypes.Name, users.Name),
                new Claim(ClaimTypes.Email,users.Email),
                new Claim(ClaimTypes.Role,role)
            };
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryInMinutes),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }

    //用户角色视图
    public class UserInfo
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public  string? Password { get; set; }
        public required string RoleName { get; set; }

    }
}
