using Xunit;
using Moq;
using AutoMapper;
using Application.Services;
using DataAccess.Repositories;
using Domain.Models;
using System.Threading;

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
            var cancellationToken = new CancellationToken();

            _mockAuthorRepository.Setup(repo => repo.GetAllAsync(cancellationToken)).ReturnsAsync(authorEntities);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<Author>>(authorEntities)).Returns(authors);

            var result = await _authorService.GetAllAuthorsAsync(cancellationToken);

            Assert.Equal(authors, result);
        }

        [Fact]
        public async Task GetAuthorByIdAsync_ReturnsAuthor()
        {
            var authorId = Guid.NewGuid();
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var author = new Author { Id = authorId, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var cancellationToken = new CancellationToken();

            _mockAuthorRepository.Setup(repo => repo.GetByIdAsync(authorId, cancellationToken)).ReturnsAsync(authorEntity);
            _mockMapper.Setup(mapper => mapper.Map<Author>(authorEntity)).Returns(author);

            var result = await _authorService.GetAuthorByIdAsync(authorId, cancellationToken);

            Assert.Equal(author, result);
        }

        [Fact]
        public async Task AddAuthorAsync_CallsAddAsyncOnRepository()
        {
            var author = new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = author.Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var cancellationToken = new CancellationToken();

            _mockMapper.Setup(mapper => mapper.Map<DataAccess.Models.AuthorEntity>(author)).Returns(authorEntity);

            await _authorService.AddAuthorAsync(author, cancellationToken);

            _mockAuthorRepository.Verify(repo => repo.AddAsync(authorEntity, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorAsync_CallsUpdateAsyncOnRepository()
        {
            var author = new Author { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var authorEntity = new DataAccess.Models.AuthorEntity { Id = author.Id, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 1, 1), Country = "USA" };
            var cancellationToken = new CancellationToken();

            _mockMapper.Setup(mapper => mapper.Map<DataAccess.Models.AuthorEntity>(author)).Returns(authorEntity);

            await _authorService.UpdateAuthorAsync(author, cancellationToken);

            _mockAuthorRepository.Verify(repo => repo.UpdateAsync(authorEntity, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorAsync_CallsDeleteAsyncOnRepository()
        {
            var authorId = Guid.NewGuid();
            var cancellationToken = new CancellationToken();

            await _authorService.DeleteAuthorAsync(authorId, cancellationToken);

            _mockAuthorRepository.Verify(repo => repo.DeleteAsync(authorId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetBooksByAuthorAsync_ReturnsListOfBooks()
        {
            var authorId = Guid.NewGuid();
            var bookEntities = new List<DataAccess.Models.BookEntity> { new DataAccess.Models.BookEntity { Id = Guid.NewGuid(), Title = "Test Book" } };
            var books = new List<Book> { new Book { Id = bookEntities[0].Id, Title = "Test Book" } };
            var cancellationToken = new CancellationToken();

            _mockAuthorRepository.Setup(repo => repo.GetBooksByAuthorAsync(authorId, cancellationToken)).ReturnsAsync(bookEntities);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<Book>>(bookEntities)).Returns(books);

            var result = await _authorService.GetBooksByAuthorAsync(authorId, cancellationToken);

            Assert.Equal(books, result);
        }
    }
}
