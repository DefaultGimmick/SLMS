using Microsoft.EntityFrameworkCore;
using SLMS.Models.Dtos.User;
using SLMS.Models.Entities;
using SLMS.Models.SLMS.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLMS.Application.Users
{
    public class UserAppService:IUserAppService
    {
        private readonly SLMSDBContext _dataContext;

        public UserAppService(SLMSDBContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ValidateUserAsync(LoginRequestDTO user)
        {
            var u = await _dataContext.Users.FirstOrDefaultAsync(c => c.UserNumber == user.UserNumber && c.Password == user.Password);
            if (u == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> RegisterUserAsync(EntityUser user)
        {
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 根据工号/学号查询用户
        /// </summary>
        /// <param name="usernumber"></param>
        /// <returns></returns>
        public async Task<EntityUser> FindUserByNameAsync(string usernumber)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync( x => x.UserNumber == usernumber);
            return user;
        }
    }
}
