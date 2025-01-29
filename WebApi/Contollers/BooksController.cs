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
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, IMapper mapper, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);

                foreach (var bookDto in booksDto)
                {
                    var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                    bookDto.ImageUrl = $"{baseImageUrl}{Path.GetFileName(bookDto.ImageUrl)}";
                }

                return Ok(booksDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllBooks");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                return BadRequest(new { Message = "Invalid book ID." });
            }

            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                var bookDto = _mapper.Map<BookDto>(book);

                var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                bookDto.ImageUrl = $"{baseImageUrl}{Path.GetFileName(book.ImagePath)}";

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBookById");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("isbn/{isbn}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBookByISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                _logger.LogError("ISBN cannot be null or empty.");
                return BadRequest(new { Message = "Invalid ISBN." });
            }

            try
            {
                var book = await _bookService.GetBookByISBNAsync(isbn);
                if (book == null)
                {
                    return NotFound();
                }
                var bookDto = _mapper.Map<BookDto>(book);

                var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                bookDto.ImageUrl = $"{baseImageUrl}{Path.GetFileName(book.ImagePath)}";

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBookByISBN");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto createBookDto)
        {
            if (createBookDto == null)
            {
                _logger.LogError("CreateBookDto cannot be null.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var book = _mapper.Map<Book>(createBookDto);
                var createdBook = await _bookService.AddBookWithAuthorCheckAsync(book);
                var bookDto = _mapper.Map<BookDto>(createdBook);

                var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                bookDto.ImageUrl = $"{baseImageUrl}{Path.GetFileName(createdBook.ImagePath)}";

                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, bookDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error in AddBook");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBook");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
        {
            if (id == Guid.Empty || updateBookDto == null)
            {
                _logger.LogError("Invalid book ID or UpdateBookDto.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var book = _mapper.Map<Book>(updateBookDto);
                await _bookService.UpdateBookAsync(book);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateBook");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid book ID.");
                return BadRequest(new { Message = "Invalid book ID." });
            }

            try
            {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteBook");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPost("{id}/borrow")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> BorrowBook(Guid id, [FromBody] BorrowBookDto borrowBookDto)
        {
            if (id == Guid.Empty || borrowBookDto == null)
            {
                _logger.LogError("Invalid book ID or BorrowBookDto.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var borrowedTime = DateTime.UtcNow;
                _logger.LogInformation($"BorrowBook: borrowedTime={borrowedTime}, dueDate={borrowBookDto.DueDate}");
                await _bookService.BorrowBookAsync(id, borrowedTime, borrowBookDto.DueDate);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BorrowBook");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }


        [HttpPost("{id}/addImage")]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddBookImage(Guid id, [FromForm] AddBookImageDto addBookImageDto)
        {
            if (id == Guid.Empty || addBookImageDto == null || addBookImageDto.Image == null)
            {
                _logger.LogError("Invalid book ID or AddBookImageDto.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var imagePath = Path.Combine(imagesPath, $"{id}_{addBookImageDto.Image.FileName}");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await addBookImageDto.Image.CopyToAsync(stream);
                }

                await _bookService.AddBookImageAsync(id, imagePath);

                var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                var imageUrl = $"{baseImageUrl}{Path.GetFileName(imagePath)}";

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBookImage");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
