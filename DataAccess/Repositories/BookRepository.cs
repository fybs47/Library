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
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(BookEntity book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
    }
}
