// Publisher-specific repository interface with specialized operations for publishers
namespace BookManagementSystem.Repositories.Interfaces
{
    public interface IPublisherRepository : IRepository<Models.Publisher>
    {
        // Get a publisher with all their published books
        Task<Models.Publisher> GetPublisherWithBooksAsync(int id);
    }
}