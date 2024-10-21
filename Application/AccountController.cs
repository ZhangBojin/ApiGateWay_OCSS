using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateWay_OCSS.Application
{
    [Route("api/[controller]")]
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
    }

    public class AccountDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
