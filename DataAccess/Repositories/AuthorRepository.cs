using DataAccess.Models;
using DataAccess.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationContext _context;
        private readonly IValidator<AuthorEntity> _authorValidator;

        public AuthorRepository(ApplicationContext context, IValidator<AuthorEntity> authorValidator)
        {
            _context = context;
            _authorValidator = authorValidator;
        }

        public async Task<IEnumerable<AuthorEntity>> GetAllAuthorsAsync(CancellationToken cancellationToken)
        {
            return await _context.Authors.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<AuthorEntity> GetAuthorByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task AddAuthorAsync(AuthorEntity author, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _authorValidator.ValidateAsync(author, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Author entity validation failed", validationResult.Errors);
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAuthorAsync(AuthorEntity author, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _authorValidator.ValidateAsync(author, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Author entity validation failed", validationResult.Errors);
            }

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAuthorAsync(Guid id, CancellationToken cancellationToken)
        {
            var author = await _context.Authors.FindAsync(new object[] { id }, cancellationToken);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId, CancellationToken cancellationToken)
        {
            return await _context.Books.AsNoTracking().Where(b => b.AuthorId == authorId).ToListAsync(cancellationToken);
        }
    }
}
