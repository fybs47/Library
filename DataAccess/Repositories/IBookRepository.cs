using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookEntity>> GetAllBooksAsync();
        Task<BookEntity> GetBookByIdAsync(Guid id);
        Task<BookEntity> GetBookByISBNAsync(string isbn);
        Task AddBookAsync(BookEntity book);
        Task UpdateBookAsync(BookEntity book);
        Task DeleteBookAsync(Guid id);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate);
        Task AddBookImageAsync(Guid id, string imagePath);
    }
}