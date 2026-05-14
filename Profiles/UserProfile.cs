using AutoMapper;
using UserServiceAPI.DTOs;
using UserServiceAPI.Entities;

namespace UserServiceAPI.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ReadUserDto>();
            CreateMap<RegisterUserDto, User>();
        }
    }
}
