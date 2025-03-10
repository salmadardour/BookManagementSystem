// Defines operations available for books in the business logic layer
using BookManagementSystem.Models;

namespace BookManagementSystem.Services.Interfaces
{
    public interface IBookService
    {
        // Get all books
        Task<IEnumerable<Book>> GetAllBooksAsync();
        
        // Get a specific book by ID
        Task<Book> GetBookByIdAsync(int id);
        
        // Get a book with all its related entities (author, category, publisher)
        Task<Book> GetBookWithDetailsAsync(int id);
        
        // Get all books by a specific author
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
        
        // Get all books in a specific category
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        
        // Get all books from a specific publisher
        Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId);
        
        // Add a new book
        Task AddBookAsync(Book book);
        
        // Update an existing book
        Task UpdateBookAsync(Book book);
        
        // Delete a book
        Task DeleteBookAsync(int id);
    }
}