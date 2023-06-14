using System;
using System.Collections.Generic;
using System.Text;

namespace SLMS.Models.Dtos.User
{
    public class RegisterRequestDTO
    {
        public string UserName { get; set; }
        public string UserNumber { get; set; }
        public string Password { get; set; }
    }
}
