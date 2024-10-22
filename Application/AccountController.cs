using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateWay_OCSS.Application
{
    [Route("ApiGateWay/Account/[action]")]
    [ApiController]
    public class AccountController(IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost]
        public async Task<ActionResult> Register(AccountDto accountDto)
        {
            try
            {
                if (!await _userRepository.AddAsync(new Users()
                    {
                        Name = accountDto.Name,
                        Email = accountDto.Email,
                        Password = accountDto.Password
                    })) throw new Exception("用户已注册！");
                await _userRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            return Ok("注册成功！");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmail(loginDto.Email);
                if (user == null) throw new Exception("用户不存在！");
                if (!await _userRepository.ValidateUserCredentialsAsync(user.Email, loginDto.Password))
                    throw new Exception("密码错误！");
                var token= _userRepository.GenerateToken(user, user.RoleName);
                return Ok($"{token}");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //[Authorize(Roles = "管理员")]
        //[HttpGet]
        //public IActionResult cs()
        //{
        //    return Ok();
        //}
    }

    public class AccountDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
