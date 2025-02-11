using AutoMapper;
using DataAccess.Models;
using Domain.Models;
using WebApi.Contracts;

namespace WebApi.MappingProfiles
{
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile()
        {
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto, Author>();

            CreateMap<Author, AuthorEntity>().ReverseMap();
        }
    }
}