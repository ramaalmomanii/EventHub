using AutoMapper;
using EventHub.Core.DTOs.Users;
using EventHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventHub.Core.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //  UserCreateDto → User
            CreateMap<UserCreateDto, User>();

            //  User → UserReadDto
            CreateMap<User, UserReadDto>();
        }
    }
}
