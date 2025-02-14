using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exсeptions;

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

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepository.GetAllAsync(cancellationToken);
            if (authors == null || !authors.Any())
            {
                throw new NotFoundException("Авторы не найдены");
            }
            return _mapper.Map<IEnumerable<Author>>(authors);
        }

        public async Task<Author> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetByIdAsync(id, cancellationToken);
            if (author == null)
            {
                throw new NotFoundException("Автор не найден");
            }
            return _mapper.Map<Author>(author);
        }

        public async Task AddAuthorAsync(Author author, CancellationToken cancellationToken)
        {
            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.AddAsync(authorEntity, cancellationToken);
            author.Id = authorEntity.Id;
        }

        public async Task UpdateAuthorAsync(Author author, CancellationToken cancellationToken)
        {
            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.UpdateAsync(authorEntity, cancellationToken);
        }

        public async Task DeleteAuthorAsync(Guid id, CancellationToken cancellationToken)
        {
            await _authorRepository.DeleteAsync(id, cancellationToken);
        }

        private string FormImageUrl(string imagePath)
        {
            return $"http://localhost:5080/images/{Path.GetFileName(imagePath)}";
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken)
        {
            var books = await _authorRepository.GetBooksByAuthorAsync(authorId, cancellationToken);
            if (books == null || !books.Any())
            {
                throw new NotFoundException("Книги не найдены");
            }

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
