using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BookManagementSystem.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<ILogger<BookService>> _mockLogger;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockLogger = new Mock<ILogger<BookService>>();
            _bookService = new BookService(_mockBookRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllBooksAsync_ReturnsAllBooks()
        {
            // Arrange
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", ISBN = "123-456789", AuthorId = 1, CategoryId = 1, PublisherId = 1 },
                new Book { Id = 2, Title = "Book 2", ISBN = "987-654321", AuthorId = 2, CategoryId = 2, PublisherId = 2 }
            };

            _mockBookRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedBooks);

            // Act
            var result = await _bookService.GetAllBooksAsync();

            // Assert
            Assert.Equal(expectedBooks.Count, result.Count());
            Assert.Equal(expectedBooks, result);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsSingleBook_WhenBookExists()
        {
            // Arrange
            var expectedBook = new Book 
            { 
                Id = 1, 
                Title = "Book 1", 
                ISBN = "123-456789", 
                AuthorId = 1, 
                CategoryId = 1, 
                PublisherId = 1 
            };

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _bookService.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBook.Id, result.Id);
            Assert.Equal(expectedBook.Title, result.Title);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsNull_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.GetBookByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddBookAsync_CallsRepositoryMethods()
        {
            // Arrange
            var book = new Book 
            { 
                Title = "New Book", 
                ISBN = "000-123456", 
                AuthorId = 1, 
                CategoryId = 1, 
                PublisherId = 1 
            };

            // Act
            await _bookService.AddBookAsync(book);

            // Assert
            _mockBookRepository.Verify(repo => repo.AddAsync(book), Times.Once);
            _mockBookRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_ThrowsException_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(99))
                .ReturnsAsync((Book)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookService.DeleteBookAsync(99));
        }
    }
}