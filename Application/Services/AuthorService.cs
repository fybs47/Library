using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper, ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                var authors = await _authorRepository.GetAllAuthorsAsync();
                return _mapper.Map<IEnumerable<Author>>(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAuthorsAsync");
                throw;
            }
        }

        public async Task<Author> GetAuthorByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                throw new ArgumentException("Invalid author ID.");
            }

            try
            {
                var author = await _authorRepository.GetAuthorByIdAsync(id);
                return _mapper.Map<Author>(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAuthorByIdAsync");
                throw;
            }
        }

        public async Task<Author> AddAuthorAsync(Author author)
        {
            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.AddAuthorAsync(authorEntity);
            author.Id = authorEntity.Id;
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            if (author == null)
            {
                _logger.LogError("Author cannot be null.");
                throw new ArgumentNullException(nameof(author));
            }

            try
            {
                var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
                await _authorRepository.UpdateAuthorAsync(authorEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAuthorAsync");
                throw;
            }
        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                throw new ArgumentException("Invalid author ID.");
            }

            try
            {
                await _authorRepository.DeleteAuthorAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAuthorAsync");
                throw;
            }
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
