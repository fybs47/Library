using AutoMapper;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorService authorService, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _authorService = authorService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var authors = await _authorService.GetAllAuthorsAsync();
                var authorsDto = _mapper.Map<IEnumerable<AuthorDto>>(authors);
                return Ok(authorsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAuthors");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                return BadRequest(new { Message = "Invalid author ID." });
            }

            try
            {
                var author = await _authorService.GetAuthorByIdAsync(id);
                if (author == null)
                {
                    return NotFound();
                }
                var authorDto = _mapper.Map<AuthorDto>(author);
                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAuthorById");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "WritePolicy")]
        public async Task<IActionResult> AddAuthor([FromBody] CreateAuthorDto createAuthorDto)
        {
            if (createAuthorDto == null)
            {
                _logger.LogError("CreateAuthorDto cannot be null.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var author = _mapper.Map<Author>(createAuthorDto);
                var createdAuthor = await _authorService.AddAuthorAsync(author);
                var authorDto = _mapper.Map<AuthorDto>(createdAuthor);
                return CreatedAtAction(nameof(GetAuthorById), new { id = createdAuthor.Id }, authorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddAuthor");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UpdatePolicy")]
        public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] UpdateAuthorDto updateAuthorDto)
        {
            if (id == Guid.Empty || updateAuthorDto == null)
            {
                _logger.LogError("Invalid author ID or UpdateAuthorDto.");
                return BadRequest(new { Message = "Invalid request." });
            }

            try
            {
                var author = _mapper.Map<Author>(updateAuthorDto);
                await _authorService.UpdateAuthorAsync(author);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAuthor");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                return BadRequest(new { Message = "Invalid author ID." });
            }

            try
            {
                await _authorService.DeleteAuthorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAuthor");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        [HttpGet("{id}/books")]
        [Authorize(Policy = "ReadPolicy")]
        public async Task<IActionResult> GetBooksByAuthor(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Invalid author ID.");
                return BadRequest(new { Message = "Invalid author ID." });
            }

            try
            {
                var books = await _authorService.GetBooksByAuthorAsync(id);
                var booksDto = _mapper.Map<IEnumerable<BookDto>>(books);

                foreach (var bookDto in booksDto)
                {
                    var baseImageUrl = $"{Request.Scheme}://{Request.Host}/images/";
                    bookDto.ImageUrl = $"{baseImageUrl}{Path.GetFileName(bookDto.ImagePath)}";
                }

                return Ok(booksDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBooksByAuthor");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
