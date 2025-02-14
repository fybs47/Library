using DataAccess.Models;
using DataAccess.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<AuthorEntity>> GetAllAuthorsAsync()
        {
            return await _context.Authors.AsNoTracking().ToListAsync();
        }

        public async Task<AuthorEntity> GetAuthorByIdAsync(Guid id)
        {
            return await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAuthorAsync(AuthorEntity author)
        {
            ValidationResult validationResult = await _authorValidator.ValidateAsync(author);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Author entity validation failed", validationResult.Errors);
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuthorAsync(AuthorEntity author)
        {
            ValidationResult validationResult = await _authorValidator.ValidateAsync(author);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Author entity validation failed", validationResult.Errors);
            }

            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuthorAsync(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BookEntity>> GetBooksByAuthorAsync(Guid authorId)
        {
            return await _context.Books.AsNoTracking().Where(b => b.AuthorId == authorId).ToListAsync();
        }
    }
}
