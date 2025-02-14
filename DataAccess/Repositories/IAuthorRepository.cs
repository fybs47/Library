using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<AuthorEntity>> GetAllAuthorsAsync(CancellationToken cancellationToken);
        Task<AuthorEntity> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAuthorAsync(AuthorEntity author, CancellationToken cancellationToken);
        Task UpdateAuthorAsync(AuthorEntity author, CancellationToken cancellationToken);
        Task DeleteAuthorAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken);
    }
}