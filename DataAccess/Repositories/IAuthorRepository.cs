using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAuthorRepository : IBaseRepository<AuthorEntity>
    {
        Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken);
    }
}