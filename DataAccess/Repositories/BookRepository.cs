using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(ApplicationContext context, ILogger<BookRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BookEntity>> GetAllBooksAsync()
        {
            _logger.LogInformation("Retrieving all books.");
            return await _context.Books.ToListAsync();
        }

        public async Task<BookEntity> GetBookByIdAsync(Guid id)
        {
            _logger.LogInformation($"Retrieving book with ID: {id}");
            return await _context.Books.FindAsync(id);
        }

        public async Task<BookEntity> GetBookByISBNAsync(string isbn)
        {
            _logger.LogInformation($"Retrieving book with ISBN: {isbn}");
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
            _logger.LogInformation($"Book added with ID: {book.Id}");
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
            _logger.LogInformation($"Deleting book with ID: {id}");
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Book deleted with ID: {id}");
            }
            else
            {
                _logger.LogWarning($"Book with ID: {id} not found.");
            }
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate)
        {
            _logger.LogInformation($"Borrowing book with ID: {id}, borrowedTime: {borrowedTime}, dueDate: {dueDate}");
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _logger.LogInformation($"Repository: Found book {id}. Updating with borrowedTime={borrowedTime}, dueDate={dueDate}");
                book.BorrowedTime = borrowedTime;
                book.DueDate = dueDate;
                book.IsBorrowed = true; // Обновляем новое свойство
                _context.Entry(book).State = EntityState.Modified;
                _logger.LogInformation($"Repository: Saving changes for book {id}");
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Repository: Changes saved for book {id}");
            }
            else
            {
                _logger.LogWarning($"Book with ID: {id} not found.");
            }
        }

        public async Task AddBookImageAsync(Guid id, string imagePath)
        {
            _logger.LogInformation($"Adding image to book with ID: {id}, imagePath: {imagePath}");
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                book.ImagePath = imagePath;
                _context.Entry(book).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Image added to book with ID: {id}");
            }
            else
            {
                _logger.LogWarning($"Book with ID: {id} not found.");
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
