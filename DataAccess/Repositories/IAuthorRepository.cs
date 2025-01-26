using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<AuthorEntity>> GetAllAuthorsAsync();
        Task<AuthorEntity> GetAuthorByIdAsync(Guid id);
        Task AddAuthorAsync(AuthorEntity author);
        Task UpdateAuthorAsync(AuthorEntity author);
        Task DeleteAuthorAsync(Guid id);
        Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId);
    }
}