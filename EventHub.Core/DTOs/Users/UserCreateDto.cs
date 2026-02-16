using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Users
{
    public class UserCreateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        
       
        //public string Role { get; set; }
    }

   
}

