using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;

namespace ApiGateWay_OCSS.Application
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HeartbeatController(IConnectionMultiplexer connectionMultiplexer) : ControllerBase
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> OnlineHeartbeat()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var redis = _connectionMultiplexer.GetDatabase(0);


            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var emailAddress = jsonToken.Claims.First(p =>
                p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;

            var tmp= await redis.StringGetAsync(emailAddress);

            if (!tmp.IsNullOrEmpty)
            {
                redis.KeyExpire(emailAddress, TimeSpan.FromMinutes(3));
                var total=await redis.ExecuteAsync("DBSIZE");
                return Ok(Convert.ToInt32(total));
            }
            else
            {
                var name = jsonToken.Claims.First(p =>
                    p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;

                await redis.StringSetAsync(emailAddress, name, TimeSpan.FromMinutes(3));
                var total =await redis.ExecuteAsync("DBSIZE");
                return Ok(Convert.ToInt32(total));
            }
        }
    }
}
