using Application.Abstractions;
using AutoMapper;
using DataAccess.Models;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Exсeptions;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly string _imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        private string FormImageUrl(string imagePath)
        {
            return $"http://localhost:5080/images/{Path.GetFileName(imagePath)}";
        }

        public async Task<string> SaveBookImageAsync(Guid id, IFormFile file)
        {
            if (!Directory.Exists(_imagesFolderPath))
            {
                Directory.CreateDirectory(_imagesFolderPath);
            }

            var fileName = $"{id}_{file.FileName}";
            var filePath = Path.Combine(_imagesFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imagePath = $"/images/{fileName}";
            await _bookRepository.AddBookImageAsync(id, imagePath);
            return FormImageUrl(imagePath);
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();
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

        public async Task<(IEnumerable<Book>, int)> GetBooksAsync(int pageNumber, int pageSize)
        {
            var (books, totalCount) = await _bookRepository.GetBooksAsync(pageNumber, pageSize);
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

            return (bookModels, totalCount);
        }

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                throw new NotFoundException("Книга не найдена");
            }
            var bookModel = _mapper.Map<Book>(book);

            if (!string.IsNullOrEmpty(bookModel.ImagePath))
            {
                bookModel.ImagePath = FormImageUrl(bookModel.ImagePath);
            }

            return bookModel;
        }

        public async Task<Book> GetBookByISBNAsync(string isbn)
        {
            var book = await _bookRepository.GetBookByISBNAsync(isbn);
            if (book == null)
            {
                throw new NotFoundException("Книга не найдена");
            }
            return _mapper.Map<Book>(book);
        }

        public async Task AddBookAsync(Book book)
        {
            if (book == null)
            {
                throw new BadRequestException("Невозможно добавить пустую книгу");
            }

            var existingBook = await _bookRepository.GetBookByIdAsync(book.Id);
            if (existingBook != null)
            {
                throw new ConflictException("Книга с таким идентификатором уже существует");
            }

            var bookEntity = _mapper.Map<BookEntity>(book);
            await _bookRepository.AddBookAsync(bookEntity);

            book.Id = bookEntity.Id;
        }

        public async Task UpdateBookAsync(Book book)
        {
            if (book == null)
            {
                throw new BadRequestException("Невозможно обновить пустую книгу");
            }

            var existingBook = await _bookRepository.GetBookByIdAsync(book.Id);
            if (existingBook == null)
            {
                throw new NotFoundException("Книга не найдена");
            }

            var bookEntity = _mapper.Map<BookEntity>(book);
            await _bookRepository.UpdateBookAsync(bookEntity);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var existingBook = await _bookRepository.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                throw new NotFoundException("Книга не найдена");
            }
            await _bookRepository.DeleteBookAsync(id);
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate)
        {
            var existingBook = await _bookRepository.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                throw new NotFoundException("Книга не найдена");
            }
            await _bookRepository.BorrowBookAsync(id, borrowedTime, dueDate);
        }

        public async Task AddBookImageAsync(Guid id, string imagePath)
        {
            var existingBook = await _bookRepository.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                throw new NotFoundException("Книга не найдена");
            }
            await _bookRepository.AddBookImageAsync(id, imagePath);
        }
    }
}
