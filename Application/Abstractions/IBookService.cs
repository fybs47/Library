using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<string> SaveBookImageAsync(Guid id, IFormFile file);
        Task<Book> GetBookByIdAsync(Guid id);
        Task<Book> GetBookByISBNAsync(string isbn);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Guid id);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate);
        Task AddBookImageAsync(Guid id, string imagePath);
    }
}