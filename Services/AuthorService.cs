// Implements business logic for author operations
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagementSystem.Services
{
    public class AuthorService : IAuthorService
    {
        // Dependencies injected through constructor
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorService> _logger;

        public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger)
        {
            _authorRepository = authorRepository;
            _logger = logger;
        }

        // Get all authors
        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all authors");
                return await _authorRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all authors");
                throw;
            }
        }

        // Get a specific author by ID
        public async Task<Author> GetAuthorByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting author with ID: {id}");
                var author = await _authorRepository.GetByIdAsync(id);
                
                if (author == null)
                {
                    _logger.LogWarning($"Author with ID: {id} was not found");
                }
                
                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author with ID: {id}");
                throw;
            }
        }

        // Get an author with all their books
        public async Task<Author> GetAuthorWithBooksAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting author with books for ID: {id}");
                var author = await _authorRepository.GetAuthorWithBooksAsync(id);
                
                if (author == null)
                {
                    _logger.LogWarning($"Author with ID: {id} was not found");
                }
                
                return author;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author with books for ID: {id}");
                throw;
            }
        }

        // Add a new author
        public async Task AddAuthorAsync(Author author)
        {
            try
            {
                _logger.LogInformation($"Adding new author: {author.Name}");
                await _authorRepository.AddAsync(author);
                await _authorRepository.SaveChangesAsync();
                _logger.LogInformation($"Author added successfully with ID: {author.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding author: {author.Name}");
                throw;
            }
        }

        // Update an existing author
        public async Task UpdateAuthorAsync(Author author)
        {
            try
            {
                _logger.LogInformation($"Updating author with ID: {author.Id}");
                await _authorRepository.UpdateAsync(author);
                await _authorRepository.SaveChangesAsync();
                _logger.LogInformation($"Author updated successfully with ID: {author.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating author with ID: {author.Id}");
                throw;
            }
        }

        // Delete an author
        public async Task DeleteAuthorAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting author with ID: {id}");
                var author = await _authorRepository.GetByIdAsync(id);
                
                if (author == null)
                {
                    _logger.LogWarning($"Author with ID: {id} not found for deletion");
                    throw new ArgumentException($"Author with ID: {id} not found");
                }
                
                await _authorRepository.DeleteAsync(author);
                await _authorRepository.SaveChangesAsync();
                _logger.LogInformation($"Author deleted successfully with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting author with ID: {id}");
                throw;
            }
        }
    }
}
