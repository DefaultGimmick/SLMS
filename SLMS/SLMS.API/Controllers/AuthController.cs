using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SLMS.Models.Entities;
using SLMS.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using SLMS.Models.Dtos.User;
using SLMS.Application.Books;
using SLMS.Application.Users;
using Microsoft.AspNetCore.Cors;
using System;
using Microsoft.Extensions.Options;
using SLMS.Tools;
using Newtonsoft.Json;
using SLMS.Infrastructure.Caching;


namespace SLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class AuthController : ControllerBase
    {

        private readonly IUserAppService _userAppService;
        private readonly IOptions<Audience> m_Settings;
        private readonly RedisContext m_redisContext;
        private readonly JwtUtils m_Token;

        public AuthController(IUserAppService userAppService, RedisContext redisContext, IOptions<Audience> settings, JwtUtils token)
        {
            _userAppService = userAppService;
            m_redisContext = redisContext;
            m_Settings = settings;
            m_Token = token;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Register(LoginRequestDTO input)
        {
            if (! await _userAppService.ValidateUserAsync(input))
            {
                return Unauthorized("Invalid username or password.");
            }
            var token = m_Token.CreateToken(input.UserNumber);
            var user = await _userAppService.FindUserByNameAsync(input.UserNumber);
            await m_redisContext.SetObjectAsync(token, user);
            await m_redisContext.SetTimeoutAsync("UserInfo", new TimeSpan(0, 15, 0));
            
            return Ok(new { access_token = token, expires_in = m_Settings.Value.TokenExpiration.TotalSeconds });
        }

        /// <summary>
        /// 注册接口
        /// </summary>
        /// <param name="input"></param>
        [HttpPost("Register")]
        public async Task UserLogin(RegisterRequestDTO input)
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
