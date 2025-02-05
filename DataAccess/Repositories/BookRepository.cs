using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationContext _context;

        public BookRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookEntity>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<BookEntity> GetBookByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<BookEntity> GetBookByISBNAsync(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        }

        public async Task AddBookAsync(BookEntity book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }
            
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateBookAsync(BookEntity book)
        {
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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded.", ex);
            }
        }
        
        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.BorrowedTime = borrowedTime;
                book.DueDate = dueDate;
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddBookImageAsync(Guid id, string imagePath)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.ImagePath = imagePath;
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<BookEntity>, int)> GetBooksAsync(int pageNumber, int pageSize)
        {
            var totalBooks = await _context.Books.CountAsync();
            var books = await _context.Books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (books, totalBooks);
        }
    }
}
