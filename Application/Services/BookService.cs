using Application.Abstractions;
using AutoMapper;
using DataAccess.Repositories;
using Domain.Models;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return _mapper.Map<IEnumerable<Book>>(books);
        }

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            return _mapper.Map<Book>(book);
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