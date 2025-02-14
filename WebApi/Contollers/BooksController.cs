using Application.Abstractions;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using System.Threading;
using Application.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IAuthorService authorService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
            _authorService = authorService;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllBooks(CancellationToken cancellationToken)
        {
            var books = await _bookService.GetAllBooksAsync(cancellationToken);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        [HttpGet("paged")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBooks(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var (books, totalCount) = await _bookService.GetBooksAsync(pageNumber, pageSize, cancellationToken);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(new { books = booksDto, totalCount });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookById(Guid id, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        [HttpGet("isbn/{isbn}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookByISBN(string isbn, CancellationToken cancellationToken)
        {
            var book = await _bookService.GetBookByISBNAsync(isbn, cancellationToken);
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        [HttpPost]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto createBookDto, CancellationToken cancellationToken)
        {
            var book = _mapper.Map<Book>(createBookDto);
            book.AuthorId = createBookDto.AuthorId;

            await _bookService.AddBookAsync(book, cancellationToken);

            var bookDto = _mapper.Map<BookDto>(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto, CancellationToken cancellationToken)
        {
            var book = _mapper.Map<Book>(updateBookDto);
            book.AuthorId = updateBookDto.AuthorId;

            await _bookService.UpdateBookAsync(book, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken)
        {
            await _bookService.DeleteBookAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/borrow")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> BorrowBook(Guid id, [FromBody] BorrowBookDto borrowBookDto, CancellationToken cancellationToken)
        {
            var borrowedTime = DateTime.UtcNow;
            await _bookService.BorrowBookAsync(id, borrowedTime, borrowBookDto.DueDate, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id}/addImage")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBookImage(Guid id, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            var imagePath = await _bookService.SaveBookImageAsync(id, file, cancellationToken);
            return Ok(new { imagePath });
        }
    }
}
