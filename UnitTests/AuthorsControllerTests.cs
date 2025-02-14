using Xunit;
using Moq;
using AutoMapper;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.Contracts;
using System.Threading;

namespace UnitTests
{
    public class AuthorsControllerTests
    {
        private readonly Mock<IAuthorService> _mockAuthorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _mockAuthorService = new Mock<IAuthorService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new AuthorsController(_mockAuthorService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAuthors_ReturnsOkResult_WithListOfAuthors()
        {
            var authors = new List<Author> { new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" } };
            var authorsDto = new List<AuthorDto> { new AuthorDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" } };
            var cancellationToken = new CancellationToken();

            _mockAuthorService.Setup(service => service.GetAllAuthorsAsync(cancellationToken)).ReturnsAsync(authors);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<AuthorDto>>(authors)).Returns(authorsDto);

            var result = await _controller.GetAllAuthors(cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAuthors = Assert.IsType<List<AuthorDto>>(okResult.Value);
            Assert.Equal(authorsDto, returnAuthors);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            var authorId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();
            _mockAuthorService.Setup(service => service.GetAuthorByIdAsync(authorId, cancellationToken)).ReturnsAsync((Author)null);

            var result = await _controller.GetAuthorById(authorId, cancellationToken);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAuthorById_ReturnsOkResult_WithAuthor()
        {
            var authorId = Guid.NewGuid();
            var author = new Author { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorDto = new AuthorDto { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var cancellationToken = new CancellationToken();

            _mockAuthorService.Setup(service => service.GetAuthorByIdAsync(authorId, cancellationToken)).ReturnsAsync(author);
            _mockMapper.Setup(mapper => mapper.Map<AuthorDto>(author)).Returns(authorDto);

            var result = await _controller.GetAuthorById(authorId, cancellationToken);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnAuthor = Assert.IsType<AuthorDto>(okResult.Value);
            Assert.Equal(authorDto, returnAuthor);
        }

        [Fact]
        public async Task AddAuthor_ReturnsCreatedAtAction_WithAuthor()
        {
            var createAuthorDto = new CreateAuthorDto { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var author = new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorDto = new AuthorDto { Id = author.Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var cancellationToken = new CancellationToken();

            _mockMapper.Setup(mapper => mapper.Map<Author>(createAuthorDto)).Returns(author);
            _mockAuthorService.Setup(service => service.AddAuthorAsync(author, cancellationToken)).Returns(Task.CompletedTask);
            _mockMapper.Setup(mapper => mapper.Map<AuthorDto>(author)).Returns(authorDto);

            var result = await _controller.AddAuthor(createAuthorDto, cancellationToken);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnAuthor = Assert.IsType<AuthorDto>(createdAtActionResult.Value);
            Assert.Equal(authorDto, returnAuthor);
        }
    }
}
