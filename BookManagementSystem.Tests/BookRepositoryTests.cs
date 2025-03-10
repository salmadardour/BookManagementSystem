using System;
using System.Linq;
using System.Threading.Tasks;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookManagementSystem.Tests
{
    public class BookRepositoryTests
    {
        private async Task<BookManagementContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<BookManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new BookManagementContext(options);
            
            context.Authors.Add(new Author { Id = 1, Name = "Test Author" });
            context.Categories.Add(new Category { Id = 1, Name = "Test Category" });
            context.Publishers.Add(new Publisher { Id = 1, Name = "Test Publisher", Address = "Test Address", ContactNumber = "123456789" });
            
            await context.SaveChangesAsync();
            
            return context;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBooks()
        {
            // Arrange
            var context = await GetDatabaseContext();
            var repository = new BookRepository(context);
            
            context.Books.Add(new Book { Id = 1, Title = "Test Book 1", ISBN = "123-456789", AuthorId = 1, CategoryId = 1, PublisherId = 1 });
            context.Books.Add(new Book { Id = 2, Title = "Test Book 2", ISBN = "987-654321", AuthorId = 1, CategoryId = 1, PublisherId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBook_WhenBookExists()
        {
            // Arrange
            var context = await GetDatabaseContext();
            var repository = new BookRepository(context);
            
            context.Books.Add(new Book { Id = 1, Title = "Test Book", ISBN = "123-456789", AuthorId = 1, CategoryId = 1, PublisherId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenBookDoesNotExist()
        {
            // Arrange
            var context = await GetDatabaseContext();
            var repository = new BookRepository(context);

            // Act
            var result = await repository.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsBookToDatabase()
        {
            // Arrange
            var context = await GetDatabaseContext();
            var repository = new BookRepository(context);
            var book = new Book { Title = "New Book", ISBN = "000-123456", AuthorId = 1, CategoryId = 1, PublisherId = 1 };

            // Act
            await repository.AddAsync(book);
            await repository.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Books.Count());
            var savedBook = await context.Books.FirstOrDefaultAsync();
            Assert.Equal("New Book", savedBook.Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesBookFromDatabase()
        {
            // Arrange
            var context = await GetDatabaseContext();
            var repository = new BookRepository(context);
            
            var book = new Book { Id = 1, Title = "Test Book", ISBN = "123-456789", AuthorId = 1, CategoryId = 1, PublisherId = 1 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(book);
            await repository.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Books.Count());
        }
    }
}