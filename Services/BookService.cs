// Implements business logic for book operations
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagementSystem.Services
{
    public class BookService : IBookService
    {
        // Dependencies injected through constructor
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        // Get all books
        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            try
            {
                _logger.LogInformation("Getting all books");
                return await _bookRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books");
                throw;
            }
        }

        // Get a specific book by ID
        public async Task<Book> GetBookByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting book with ID: {id}");
                var book = await _bookRepository.GetByIdAsync(id);
                
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {id} was not found");
                }
                
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book with ID: {id}");
                throw;
            }
        }

        // Get a book with all its related entities (author, category, publisher)
        public async Task<Book> GetBookWithDetailsAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting book with details for ID: {id}");
                var book = await _bookRepository.GetBookWithDetailsAsync(id);
                
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {id} was not found");
                }
                
                return book;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving book details with ID: {id}");
                throw;
            }
        }

        // Get all books by a specific author
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            try
            {
                _logger.LogInformation($"Getting books by author ID: {authorId}");
                return await _bookRepository.GetBooksByAuthorAsync(authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving books by author ID: {authorId}");
                throw;
            }
        }

        // Get all books in a specific category
        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            try
            {
                _logger.LogInformation($"Getting books by category ID: {categoryId}");
                return await _bookRepository.GetBooksByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving books by category ID: {categoryId}");
                throw;
            }
        }

        // Get all books from a specific publisher
        public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId)
        {
            try
            {
                _logger.LogInformation($"Getting books by publisher ID: {publisherId}");
                return await _bookRepository.GetBooksByPublisherAsync(publisherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving books by publisher ID: {publisherId}");
                throw;
            }
        }

        // Add a new book
        public async Task AddBookAsync(Book book)
        {
            try
            {
                _logger.LogInformation($"Adding new book: {book.Title}");
                await _bookRepository.AddAsync(book);
                await _bookRepository.SaveChangesAsync();
                _logger.LogInformation($"Book added successfully with ID: {book.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding book: {book.Title}");
                throw;
            }
        }

        // Update an existing book
        public async Task UpdateBookAsync(Book book)
        {
            try
            {
                _logger.LogInformation($"Updating book with ID: {book.Id}");
                await _bookRepository.UpdateAsync(book);
                await _bookRepository.SaveChangesAsync();
                _logger.LogInformation($"Book updated successfully with ID: {book.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID: {book.Id}");
                throw;
            }
        }

        // Delete a book
        public async Task DeleteBookAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting book with ID: {id}");
                var book = await _bookRepository.GetByIdAsync(id);
                
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {id} not found for deletion");
                    throw new ArgumentException($"Book with ID: {id} not found");
                }
                
                await _bookRepository.DeleteAsync(book);
                await _bookRepository.SaveChangesAsync();
                _logger.LogInformation($"Book deleted successfully with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID: {id}");
                throw;
            }
        }
    }
}