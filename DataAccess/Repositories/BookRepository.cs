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
    public class BookRepository : BaseRepository<BookEntity>, IBookRepository
    {
        private readonly IValidator<BookEntity> _bookValidator;

        public BookRepository(ApplicationContext context, IValidator<BookEntity> bookValidator)
            : base(context)
        {
            _bookValidator = bookValidator;
        }

        public async Task<BookEntity> GetBookByISBNAsync(string isbn, CancellationToken cancellationToken)
        {
            return await Context.Set<BookEntity>().AsNoTracking().FirstOrDefaultAsync(b => b.ISBN == isbn, cancellationToken);
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate, CancellationToken cancellationToken)
        {
            var book = await Context.Set<BookEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                book.BorrowedTime = borrowedTime;
                book.DueDate = dueDate;
                Context.Entry(book).State = EntityState.Modified;
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task AddBookImageAsync(Guid id, string imagePath, CancellationToken cancellationToken)
        {
            var book = await Context.Set<BookEntity>().FindAsync(new object[] { id }, cancellationToken);
            if (book != null)
            {
                ValidationResult validationResult = _bookValidator.Validate(book);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException("Book entity validation failed", validationResult.Errors);
                }

                book.ImagePath = imagePath;
                Context.Entry(book).State = EntityState.Modified;
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<(IEnumerable<BookEntity>, int)> GetBooksAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var totalBooks = await Context.Set<BookEntity>().CountAsync(cancellationToken);
            var books = await Context.Set<BookEntity>().AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (books, totalBooks);
        }
    }
}
