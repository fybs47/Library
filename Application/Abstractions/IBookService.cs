using Domain.Models;

namespace Application.Abstractions
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book> GetBookByIdAsync(Guid id);
        Task<Book> GetBookByISBNAsync(string isbn);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Guid id);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate);
        Task<Book> AddBookWithAuthorCheckAsync(Book book);
        Task AddBookImageAsync(Guid id, string imagePath);
    }
}