using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookEntity>> GetAllBooksAsync(CancellationToken cancellationToken);
        Task<BookEntity> GetBookByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BookEntity> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken);
        Task AddBookAsync(BookEntity book, CancellationToken cancellationToken);
        Task UpdateBookAsync(BookEntity book, CancellationToken cancellationToken);
        Task DeleteBookAsync(Guid id, CancellationToken cancellationToken);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate, CancellationToken cancellationToken);
        Task AddBookImageAsync(Guid id, string imagePath, CancellationToken cancellationToken);
        Task<(IEnumerable<BookEntity>, int)> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}