using AutoMapper;
using DataAccess.Models;
using Domain.Models;
using WebApi.Contracts;

namespace WebApi.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserEntity>().ReverseMap();
            CreateMap<RegisterUserDto, User>();
        }
    }
}