using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBookRepository : IBaseRepository<BookEntity>
    {
        Task<BookEntity> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken);
        Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate, CancellationToken cancellationToken);
        Task AddBookImageAsync(Guid id, string imagePath, CancellationToken cancellationToken);
        Task<(IEnumerable<BookEntity>, int)> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}