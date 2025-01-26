using AutoMapper;
using Domain.Models;
using WebApi.Contracts;
using DataAccess.Models;

namespace WebApi.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
            CreateMap<BorrowBookDto, Book>();
            CreateMap<AddBookImageDto, Book>();

            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto, Author>();

            CreateMap<Author, AuthorEntity>().ReverseMap();
            CreateMap<Book, BookEntity>().ReverseMap();
        }
    }
}