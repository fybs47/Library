using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            if (authors == null || !authors.Any())
            {
                throw new NotFoundException("Авторы не найдены");
            }
            return _mapper.Map<IEnumerable<Author>>(authors);
        }

        public async Task<Author> GetAuthorByIdAsync(Guid id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null)
            {
                throw new NotFoundException("Автор не найден");
            }
            return _mapper.Map<Author>(author);
        }

        public async Task AddAuthorAsync(Author author)
        {
            var existingAuthor = await _authorRepository.GetAuthorByIdAsync(author.Id);
            if (existingAuthor != null)
            {
                throw new ConflictException("Автор с таким идентификатором уже существует");
            }

            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.AddAuthorAsync(authorEntity);
            author.Id = authorEntity.Id;
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            var existingAuthor = await _authorRepository.GetAuthorByIdAsync(author.Id);
            if (existingAuthor == null)
            {
                throw new NotFoundException("Автор не найден");
            }

            var authorEntity = _mapper.Map<DataAccess.Models.AuthorEntity>(author);
            await _authorRepository.UpdateAuthorAsync(authorEntity);
        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            var existingAuthor = await _authorRepository.GetAuthorByIdAsync(id);
            if (existingAuthor == null)
            {
                throw new NotFoundException("Автор не найден");
            }
            await _authorRepository.DeleteAuthorAsync(id);
        }

        private string FormImageUrl(string imagePath)
        {
            return $"http://localhost:5080/images/{Path.GetFileName(imagePath)}";
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
            if (author == null)
            {
                throw new NotFoundException("Автор не найден");
            }

            var books = await _authorRepository.GetBooksByAuthorAsync(authorId);
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