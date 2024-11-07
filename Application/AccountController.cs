using ApiGateWay_OCSS.Domain.Entities;
using ApiGateWay_OCSS.Domain.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using ApiGateWay_OCSS.Infrastructure.EfCore;
using ApiGateWay_OCSS.Infrastructure.RabbitMq;
using ApiGateWay_OCSS.Infrastructure.Repositories;

namespace ApiGateWay_OCSS.Application
{
    [Route("ApiGateWay/Account/[action]")]
    [ApiController]
    public class AccountController(IUserRepository userRepository, IMenuInfoRepository menuInfoRepository
                                    , IRoleRepository roleRepository,
                                    RabbitMqProducer mqProducer) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMenuInfoRepository _menuInfoRepository= menuInfoRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly RabbitMqProducer _mqProducer = mqProducer;

        [HttpPost]
        public async Task<ActionResult> Register(AccountDto accountDto)
        {
            try
            {
                const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(accountDto.Email, pattern)) throw new Exception("不是有效邮箱！");

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
            _mqProducer.Log(new UserInfo(), "AccountController", "Login", "接口被调用！", "Info");

            try
            {
                var user = await _userRepository.GetByEmail(loginDto.Email);
                if (user == null)
                {
                    return NotFound("用户不存在！");
                }

                var isValidCredentials = await _userRepository.ValidateUserCredentialsAsync(user.Email!, loginDto.Password);
                if (!isValidCredentials)
                {
                    return Unauthorized("密码错误！");
                }

                var token = _userRepository.GenerateToken(user, user.RoleName!);
                var roleId =  _roleRepository.GetRoleId(user.RoleName!);
                var menu = await _menuInfoRepository.GetMenu(roleId);

                return Ok(new
                {
                    token = token,
                    menu = menu
                });
            }
            catch (Exception ex)
            {
                _mqProducer.Log(new UserInfo(), "AccountController", "action", $"接口抛出异常：{ex.Message}", "warning");
                return StatusCode(500, "服务器内部错误，请稍后再试。");
            }
        }


        #region test
        //[Authorize(Roles = "管理员")]
        //[HttpGet]
        //public IActionResult cs()
        //{
        //    return Ok();
        //}

        //[HttpPost]
        //public ActionResult LogTest()
        //{
        //    _mqProducer.Log(new UserInfo(), "AccountController", "action","测试","Info");
        //    return Ok();
        //}
        #endregion
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
