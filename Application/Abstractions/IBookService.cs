using Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(CancellationToken cancellationToken);
        Task<(IEnumerable<Book>, int)> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<string> SaveBookImageAsync(Guid id, IFormFile file, CancellationToken cancellationToken);
        Task<Book> GetBookByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Book> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken);
        Task AddBookAsync(Book book, CancellationToken cancellationToken);
        Task UpdateBookAsync(Book book, CancellationToken cancellationToken);
        Task DeleteBookAsync(Guid id, CancellationToken cancellationToken);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate, CancellationToken cancellationToken);
        Task AddBookImageAsync(Guid id, string imagePath, CancellationToken cancellationToken);
    }
}