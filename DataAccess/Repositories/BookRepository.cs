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
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationContext _context;
        private readonly IValidator<BookEntity> _bookValidator;

        public BookRepository(ApplicationContext context, IValidator<BookEntity> bookValidator)
        {
            _context = context;
            _bookValidator = bookValidator;
        }

        public async Task<IEnumerable<BookEntity>> GetAllBooksAsync(CancellationToken cancellationToken)
        {
            return await _context.Books.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<BookEntity> GetBookByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<BookEntity> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken)
        {
            return await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
        }

        public async Task AddBookAsync(BookEntity book, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _bookValidator.ValidateAsync(book, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Book entity validation failed", validationResult.Errors);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateBookAsync(BookEntity book, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _bookValidator.ValidateAsync(book, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Book entity validation failed", validationResult.Errors);
            }

            var local = _context.Set<BookEntity>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(book.Id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(book);
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded.", ex);
            }
        }

        public async Task DeleteBookAsync(Guid id, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                book.BorrowedTime = borrowedTime;
                book.DueDate = dueDate;
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task AddBookImageAsync(Guid id, string imagePath, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                ValidationResult validationResult = _bookValidator.Validate(book);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException("Book entity validation failed", validationResult.Errors);
                }

                book.ImagePath = imagePath;
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<(IEnumerable<BookEntity>, int)> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var totalBooks = await _context.Books.CountAsync(cancellationToken);
            var books = await _context.Books.AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (books, totalBooks);
        }
    }
}
