using AutoMapper;
using DataAccess.Models;
using Domain.Models;
using WebApi.Contracts;

namespace WebApi.MappingProfiles
{
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем навигационное свойство

            CreateMap<UpdateBookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())  // Игнорируем Id
                .ForMember(dest => dest.Author, opt => opt.Ignore());  // Игнорируем сущность автора, но НЕ AuthorId

            CreateMap<BorrowBookDto, Book>().ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем навигационное свойство при заимствовании
            CreateMap<AddBookImageDto, Book>().ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем навигационное свойство при добавлении изображения

            CreateMap<Book, BookEntity>().ReverseMap()
                .ForMember(dest => dest.Author, opt => opt.Ignore()); // Игнорируем навигационное свойство
        }
    }
}