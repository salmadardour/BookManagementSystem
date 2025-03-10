// Defines operations available for authors in the business logic layer
using BookManagementSystem.Models;

namespace BookManagementSystem.Services.Interfaces
{
    public interface IAuthorService
    {
        // Get all authors
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        
        // Get a specific author by ID
        Task<Author> GetAuthorByIdAsync(int id);
        
        // Get an author with all their books
        Task<Author> GetAuthorWithBooksAsync(int id);
        
        // Add a new author
        Task AddAuthorAsync(Author author);
        
        // Update an existing author
        Task UpdateAuthorAsync(Author author);
        
        // Delete an author
        Task DeleteAuthorAsync(int id);
    }
}