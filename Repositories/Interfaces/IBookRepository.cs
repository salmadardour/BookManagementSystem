// Book-specific repository interface with specialized operations for books
namespace BookManagementSystem.Repositories.Interfaces
{
    public interface IBookRepository : IRepository<Models.Book>
    {
        // Get all books by a specific author
        Task<IEnumerable<Models.Book>> GetBooksByAuthorAsync(int authorId);
        
        // Get all books in a specific category
        Task<IEnumerable<Models.Book>> GetBooksByCategoryAsync(int categoryId);
        
        // Get all books from a specific publisher
        Task<IEnumerable<Models.Book>> GetBooksByPublisherAsync(int publisherId);
        
        // Get a book with all its related entities (author, category, publisher)
        Task<Models.Book> GetBookWithDetailsAsync(int id);
    }
}