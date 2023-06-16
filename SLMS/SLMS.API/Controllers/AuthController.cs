using Microsoft.AspNetCore.Mvc;
using SLMS.Models.Entities;
using System.Threading.Tasks;
using SLMS.Models.Dtos.User;
using SLMS.Application.Users;
using Microsoft.AspNetCore.Cors;
using System;
using Microsoft.Extensions.Options;
using SLMS.Tools;
using SLMS.Infrastructure.Caching;


namespace SLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class AuthController : ControllerBase
    {

        private readonly IUserAppService _userAppService;
        private readonly IOptions<Audience> _settings;
        private readonly RedisContext _redisContext;
        private readonly JwtUtils _jwtUtils;

        public AuthController(IUserAppService userAppService, RedisContext redisContext, IOptions<Audience> settings, JwtUtils jwtUtils)
        {
            _userAppService = userAppService;
            _redisContext = redisContext;
            _settings = settings;
            _jwtUtils = jwtUtils;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(LoginRequestDTO input)
        {
            if (! await _userAppService.ValidateUserAsync(input))
            {
                return Unauthorized("Invalid username or password.");
            }
            var token = _jwtUtils.CreateToken(input.UserNumber);
            var user = await _userAppService.FindUserByNameAsync(input.UserNumber);
            await _redisContext.SetObjectAsync(token, user);
            await _redisContext.SetTimeoutAsync("UserInfo", new TimeSpan(0, 15, 0));
            
            return Ok(new { access_token = token, expires_in = _settings.Value.TokenExpiration.TotalSeconds });
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("Register")]
        public async Task UserRegister(RegisterRequestDTO input)
        {
            var user = new EntityUser
            {
                UserNumber = input.UserNumber,
                UserName = input.UserName,
                Password = input.Password,
                UserType = 2
            };
           await  _userAppService.RegisterUserAsync(user);
        }
    }
}
