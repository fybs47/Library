using Application.Abstractions;
using Application.Services;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Contracts;

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

        public BooksController(IBookService bookService, IAuthorService authorService, IMapper mapper )
        {
            _bookService = bookService;
            _mapper = mapper;
            _authorService = authorService;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(booksDto);
        }

        [HttpGet("paged")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBooks(int pageNumber, int pageSize)
        {
            var (books, totalCount) = await _bookService.GetBooksAsync(pageNumber, pageSize);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);
            return Ok(new { books = booksDto, totalCount });
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
            if (createBookDto == null)
            {
                return BadRequest("Некорректные данные.");
            }

            var book = _mapper.Map<Book>(createBookDto);

            if (book == null)
            {
                return BadRequest("Ошибка маппинга данных.");
            }

            var authorExists = await _authorService.GetAuthorByIdAsync(createBookDto.AuthorId);
            if (authorExists == null)
            {
                return BadRequest("Указанный автор не существует.");
            }

            book.AuthorId = createBookDto.AuthorId;

            await _bookService.AddBookAsync(book);

            if (book.Id == Guid.Empty)
            {
                return BadRequest("Ошибка сохранения книги.");
            }

            var bookDto = _mapper.Map<BookDto>(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, bookDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id == Guid.Empty || updateBookDto == null)
            {
                return BadRequest("Некорректные данные.");
            }

            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            var author = await _authorService.GetAuthorByIdAsync(updateBookDto.AuthorId);
            if (author == null)
            {
                return BadRequest("Указанный автор не существует.");
            }

            _mapper.Map(updateBookDto, book);

                    book.AuthorId = updateBookDto.AuthorId;

            try
            {
                await _bookService.UpdateBookAsync(book);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(book); 
        }

        private async Task<bool> BookExists(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return book != null;
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
        public async Task<IActionResult> AddBookImage(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не загружен.");
            }

            var imagePath = await _bookService.SaveBookImageAsync(id, file);
            return Ok(new { imagePath });
        }
    }
}
