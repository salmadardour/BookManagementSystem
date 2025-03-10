using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookManagementSystem.Controllers;
using BookManagementSystem.DTO;
using BookManagementSystem.Models;
using BookManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BookManagementSystem.Tests
{
    public class BooksControllerTests
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<BooksController> _logger;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<BookManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new BookManagementContext(options);
            
            // Set up test data
            _context.Authors.Add(new Author { Id = 1, Name = "Test Author" });
            _context.Categories.Add(new Category { Id = 1, Name = "Test Category" });
            _context.Publishers.Add(new Publisher { Id = 1, Name = "Test Publisher", Address = "Test Address", ContactNumber = "123456789" });
            _context.SaveChanges();
            
            // Create a real logger using LoggerFactory
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<BooksController>();
            
            // Create the controller with real context and logger
            _controller = new BooksController(_context, _logger);
        }

        [Fact]
        public async Task GetBooks_ReturnsOkResult_WithListOfBooks()
        {
            // Arrange
            _context.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<BookDTO>>>(result);
            var books = Assert.IsAssignableFrom<IEnumerable<BookDTO>>(actionResult.Value);
            Assert.NotEmpty(books);
        }

        [Fact]
        public async Task GetBook_ReturnsOkResult_WhenBookExists()
        {
            // Arrange
            _context.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<BookDTO>>(result);
            var book = Assert.IsType<BookDTO>(actionResult.Value);
            Assert.Equal("Test Book", book.Title);
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Act
            var result = await _controller.GetBook(99);

            // Assert
            var actionResult = Assert.IsType<ActionResult<BookDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostBook_CreatesBook_WhenModelStateIsValid()
        {
            // Arrange
            var bookDTO = new BookDTO
            {
                Title = "New Book",
                ISBN = "000-123456",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1,
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                PublisherName = "Test Publisher",
                PublisherAddress = "Test Address",
                PublisherContactNumber = "123456789"
            };

            // Act
            var result = await _controller.PostBook(bookDTO);

            // Assert
            var actionResult = Assert.IsType<ActionResult<BookDTO>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetBook", createdAtActionResult.ActionName);
            
            // Verify book was added to database
            var bookInDb = await _context.Books.FindAsync(((BookDTO)createdAtActionResult.Value).Id);
            Assert.NotNull(bookInDb);
            Assert.Equal("New Book", bookInDb.Title);
        }

        [Fact]
        public async Task PutBook_UpdatesBook_WhenBookExists()
        {
            // Arrange
            _context.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1
            });
            await _context.SaveChangesAsync();

            var updatedBookDTO = new BookDTO
            {
                Id = 1,
                Title = "Updated Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1,
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                PublisherName = "Test Publisher",
                PublisherAddress = "Test Address",
                PublisherContactNumber = "123456789"
            };

            // Act
            var result = await _controller.PutBook(1, updatedBookDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify book was updated in database
            var bookInDb = await _context.Books.FindAsync(1);
            Assert.Equal("Updated Book", bookInDb.Title);
        }

        [Fact]
        public async Task PutBook_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var bookDTO = new BookDTO
            {
                Id = 2,
                Title = "Updated Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1,
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                PublisherName = "Test Publisher",
                PublisherAddress = "Test Address",
                PublisherContactNumber = "123456789"
            };

            // Act
            var result = await _controller.PutBook(1, bookDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var bookDTO = new BookDTO
            {
                Id = 99,
                Title = "Updated Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1,
                AuthorName = "Test Author",
                CategoryName = "Test Category",
                PublisherName = "Test Publisher",
                PublisherAddress = "Test Address",
                PublisherContactNumber = "123456789"
            };

            // Act
            var result = await _controller.PutBook(99, bookDTO);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            _context.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                ISBN = "123-456789",
                AuthorId = 1,
                CategoryId = 1,
                PublisherId = 1
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Verify book was removed from database
            var bookInDb = await _context.Books.FindAsync(1);
            Assert.Null(bookInDb);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteBook(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}