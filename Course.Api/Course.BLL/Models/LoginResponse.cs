using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public bool IsCreatingPost { get; set; }
        public string Token { get; set; }
    }
}
