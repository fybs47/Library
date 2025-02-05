using Application.Abstractions;
using AutoMapper;
using DataAccess.Models;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;
        private readonly string _imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        private string FormImageUrl(string imagePath)
        {
            return $"http://localhost:5080/images/{Path.GetFileName(imagePath)}";
        }

        public async Task<string> SaveBookImageAsync(Guid id, IFormFile file)
        {
            if (!Directory.Exists(_imagesFolderPath))
            {
                Directory.CreateDirectory(_imagesFolderPath);
            }

            var fileName = $"{id}_{file.FileName}";
            var filePath = Path.Combine(_imagesFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imagePath = $"/images/{fileName}";
            await _bookRepository.AddBookImageAsync(id, imagePath);
            return FormImageUrl(imagePath);
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            var bookModels = _mapper.Map<IEnumerable<Book>>(books);

            foreach (var book in bookModels)
            {
                if (!string.IsNullOrEmpty(book.ImagePath))
                {
                    book.ImagePath = FormImageUrl(book.ImagePath);
                }
            }

            return bookModels;
        }

        public async Task<(IEnumerable<Book>, int)> GetBooksAsync(int pageNumber, int pageSize)
        {
            var (books, totalCount) = await _bookRepository.GetBooksAsync(pageNumber, pageSize);
            var bookModels = _mapper.Map<IEnumerable<Book>>(books);

            foreach (var book in bookModels)
            {
                if (!string.IsNullOrEmpty(book.ImagePath))
                {
                    book.ImagePath = FormImageUrl(book.ImagePath);
                }
            }

            return (bookModels, totalCount);
        }

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                throw new ArgumentException("Invalid book ID.");
            }

            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                return _mapper.Map<Book>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBookByIdAsync");
                throw;
            }
        }

        public async Task<Book> GetBookByISBNAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                _logger.LogError("ISBN cannot be null or empty.");
                throw new ArgumentException("ISBN cannot be null or empty.");
            }

            try
            {
                var book = await _bookRepository.GetBookByISBNAsync(isbn);
                return _mapper.Map<Book>(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBookByISBNAsync");
                throw;
            }
        }

        public async Task AddBookAsync(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            var bookEntity = _mapper.Map<BookEntity>(book);
            await _bookRepository.AddBookAsync(bookEntity);

            book.Id = bookEntity.Id;
        }

        public async Task UpdateBookAsync(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            var bookEntity = _mapper.Map<BookEntity>(book);
            await _bookRepository.UpdateBookAsync(bookEntity);
        }


        public async Task DeleteBookAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                throw new ArgumentException("Invalid book ID.");
            }

            try
            {
                await _bookRepository.DeleteBookAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteBookAsync");
                throw;
            }
        }

        public async Task BorrowBookAsync(Guid id, DateTime borrowedTime, DateTime dueDate)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                throw new ArgumentException("Invalid book ID.");
            }

            if (borrowedTime == default || dueDate == default || borrowedTime >= dueDate)
            {
                _logger.LogError("Invalid borrowed time or due date.");
                throw new ArgumentException("Invalid borrowed time or due date.");
            }

            _logger.LogInformation($"Service: BorrowBookAsync called with borrowedTime={borrowedTime}, dueDate={dueDate}");
            try
            {
                await _bookRepository.BorrowBookAsync(id, borrowedTime, dueDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BorrowBookAsync");
                throw;
            }
        }

        
        public async Task<Book> AddBookWithAuthorCheckAsync(Book book)
        {
            if (book == null)
            {
                _logger.LogError("Book cannot be null.");
                throw new ArgumentNullException(nameof(book));
            }

            if (book.AuthorId == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                throw new ArgumentException("Invalid author ID.");
            }

            // Проверяем, существует ли автор
            var author = await _authorRepository.GetAuthorByIdAsync(book.AuthorId);
            if (author == null)
            {
                _logger.LogError("Author not found.");
                throw new ArgumentException("Author not found.");
            }

            try
            {
                var bookEntity = _mapper.Map<DataAccess.Models.BookEntity>(book);
                await _bookRepository.AddBookAsync(bookEntity);
                return _mapper.Map<Book>(bookEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBookWithAuthorCheckAsync");
                throw;
            }
        }

        
        public async Task AddBookImageAsync(Guid id, string imagePath)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                throw new ArgumentException("Invalid book ID.");
            }

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                _logger.LogError("Image path cannot be null or empty.");
                throw new ArgumentException("Image path cannot be null or empty.");
            }

            try
            {
                await _bookRepository.AddBookImageAsync(id, imagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBookImageAsync");
                throw;
            }
        }
    }
}
