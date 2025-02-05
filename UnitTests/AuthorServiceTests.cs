using Xunit;
using Moq;
using AutoMapper;
using Application.Services;
using DataAccess.Repositories;
using Domain.Models;

namespace Application.Tests
{
    public class AuthorServiceTests
    {
        private readonly Mock<IAuthorRepository> _mockAuthorRepository;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AuthorService _authorService;

        public AuthorServiceTests()
        {
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _authorService = new AuthorService(_mockAuthorRepository.Object, _mockBookRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAuthorsAsync_ReturnsListOfAuthors()
        {
            var authorEntities = new List<DataAccess.Models.AuthorEntity> { new DataAccess.Models.AuthorEntity { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" } };
            var authors = new List<Author> { new Author { Id = authorEntities[0].Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" } };

            _mockAuthorRepository.Setup(repo => repo.GetAllAuthorsAsync()).ReturnsAsync(authorEntities);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<Author>>(authorEntities)).Returns(authors);

            var result = await _authorService.GetAllAuthorsAsync();

            Assert.Equal(authors, result);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ReturnsAuthor()
        {
            var authorId = Guid.NewGuid();
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var author = new Author { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };

            _mockAuthorRepository.Setup(repo => repo.GetAuthorByIdAsync(authorId)).ReturnsAsync(authorEntity);
            _mockMapper.Setup(mapper => mapper.Map<Author>(authorEntity)).Returns(author);

            var result = await _authorService.GetAuthorByIdAsync(authorId);

            Assert.Equal(author, result);
        }

        [Fact]
        public async Task AddAuthorAsync_CallsAddAuthorAsyncOnRepository()
        {
            var author = new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = author.Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };

            _mockMapper.Setup(mapper => mapper.Map<DataAccess.Models.AuthorEntity>(author)).Returns(authorEntity);

            await _authorService.AddAuthorAsync(author);

            _mockAuthorRepository.Verify(repo => repo.AddAuthorAsync(authorEntity), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_CallsUpdateAuthorAsyncOnRepository()
        {
            var author = new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = author.Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };

            _mockMapper.Setup(mapper => mapper.Map<DataAccess.Models.AuthorEntity>(author)).Returns(authorEntity);

            await _authorService.UpdateAuthorAsync(author);

            _mockAuthorRepository.Verify(repo => repo.UpdateAuthorAsync(authorEntity), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_CallsDeleteAuthorAsyncOnRepository()
        {
            var authorId = Guid.NewGuid();

            await _authorService.DeleteAuthorAsync(authorId);

            _mockAuthorRepository.Verify(repo => repo.DeleteAuthorAsync(authorId), Times.Once);
        }

        [Fact]
        public async Task GetBooksByAuthorAsync_ReturnsListOfBooks()
        {
            var authorId = Guid.NewGuid();
            var bookEntities = new List<DataAccess.Models.BookEntity> { new DataAccess.Models.BookEntity { Id = Guid.NewGuid(), Title = "Test Book" } };
            var books = new List<Book> { new Book { Id = bookEntities[0].Id, Title = "Test Book" } };

            _mockAuthorRepository.Setup(repo => repo.GetBooksByAuthorAsync(authorId)).ReturnsAsync(bookEntities);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<Book>>(bookEntities)).Returns(books);

            var result = await _authorService.GetBooksByAuthorAsync(authorId);

            Assert.Equal(books, result);
        }
    }
}
