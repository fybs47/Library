using Application.Abstractions;
using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
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
            return _mapper.Map<Book>(book);
        }

        public async Task AddBookAsync(Book book)
        {
            var bookEntity = _mapper.Map<DataAccess.Models.BookEntity>(book);
            await _bookRepository.AddBookAsync(bookEntity);
        }

        public async Task UpdateBookAsync(Book book)
        {
            var bookEntity = _mapper.Map<DataAccess.Models.BookEntity>(book);
            await _bookRepository.UpdateBookAsync(bookEntity);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            await _bookRepository.DeleteBookAsync(id);
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate)
        {
            await _bookRepository.BorrowBookAsync(id, borrowedTime, dueDate);
        }

        public async Task AddBookImageAsync(Guid id, string imagePath)
        {
            await _bookRepository.AddBookImageAsync(id, imagePath);
        }
    }
    
    
}