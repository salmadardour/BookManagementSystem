// Author-specific repository interface with specialized operations for authors
namespace BookManagementSystem.Repositories.Interfaces
{
    public interface IAuthorRepository : IRepository<Models.Author>
    {
        // Get an author with all their books
        Task<Models.Author> GetAuthorWithBooksAsync(int id);
    }
}