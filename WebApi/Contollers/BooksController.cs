using Application.Abstractions;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public BooksController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        [HttpGet("isbn/{isbn}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookByISBN(string isbn)
        {
            var book = await _bookService.GetBookByISBNAsync(isbn);
            if (book == null)
            {
                return NotFound();
            }
            var bookDto = _mapper.Map<BookDto>(book);
            return Ok(bookDto);
        }

        [HttpPost]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto createBookDto)
        {
            var book = _mapper.Map<Book>(createBookDto);
            await _bookService.AddBookAsync(book);
            var bookDto = _mapper.Map<BookDto>(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
            {
                return BadRequest();
            }

            var book = _mapper.Map<Book>(updateBookDto);
            await _bookService.UpdateBookAsync(book);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/borrow")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> BorrowBook(Guid id, [FromBody] BorrowBookDto borrowBookDto)
        {
            if (id != borrowBookDto.BookId)
            {
                return BadRequest();
            }

            var borrowedTime = DateTime.UtcNow;
            await _bookService.BorrowBookAsync(id, borrowedTime, borrowBookDto.DueDate);
            return NoContent();
        }

        [HttpPost("{id}/addImage")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBookImage(Guid id, [FromBody] AddBookImageDto addBookImageDto)
        {
            if (id != addBookImageDto.BookId)
            {
                return BadRequest();
            }

            await _bookService.AddBookImageAsync(id, addBookImageDto.ImagePath);
            return NoContent();
        }
    }
}
