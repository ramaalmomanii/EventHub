using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Core.DTOs.Users
{
    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string? Status { get; set; }
    }

    public class AdminUpdateUserDto
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}
