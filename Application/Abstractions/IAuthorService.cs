using Domain.Models;

namespace Application.Services
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author> GetAuthorByIdAsync(Guid id);
        Task AddAuthorAsync(Author author);
        Task UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(Guid id);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(Guid authorId);
    }
}