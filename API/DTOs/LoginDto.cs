using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LoginDto
    {
        // Class for expected format of logging in users
        public string Email { get; set; }
        public string Password { get; set; }
    }
}