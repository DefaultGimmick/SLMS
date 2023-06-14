using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Dtos.User
{
    public class LoginResponseDTO
    {
        /// <summary>
        /// 学号/工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
