using SLMS.Models.Dtos.User;
using SLMS.Models.Entities;
using System.Threading.Tasks;

namespace SLMS.Application.Users
{
    public interface IUserAppService
    {
        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> ValidateUserAsync(LoginRequestDTO user);

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> RegisterUserAsync(EntityUser user);

        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="userNumber"></param>
        /// <returns></returns>
        public Task<EntityUser> FindUserByNameAsync(string userNumber);

    }
}
