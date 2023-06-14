using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Dtos.User
{
    public class LoginRequestDTO
    {
        public string UserNumber { get; set; }

        public string Password { get; set; }
    }
}
