using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;

namespace Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return _mapper.Map<IEnumerable<Author>>(authors);
        }

        public async Task<Author> GetAuthorByIdAsync(Guid id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            return _mapper.Map<Author>(author);
        }

        public async Task AddAuthorAsync(Author author)
        {
            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.AddAuthorAsync(authorEntity);
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.UpdateAuthorAsync(authorEntity);
        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            await _authorRepository.DeleteAuthorAsync(id);
        }

        private string FormImageUrl(string imagePath)
        {
            return $"http://localhost:5080/images/{Path.GetFileName(imagePath)}";
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId)
        {
            var books = await _authorRepository.GetBooksByAuthorAsync(authorId);
            var bookModels = _mapper.Map<IEnumerable<Book>>(books);

            foreach (var book in bookModels)
            {
                if (!string.IsNullOrEmpty(book.ImagePath))
                {
                    book.ImagePath = FormImageUrl(book.ImagePath);
                }
            }

            return bookModels;
        }
    }
}