using Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync(CancellationToken cancellationToken);
        Task<Author> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddAuthorAsync(Author author, CancellationToken cancellationToken);
        Task UpdateAuthorAsync(Author author, CancellationToken cancellationToken);
        Task DeleteAuthorAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken);
    }
}