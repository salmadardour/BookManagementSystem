// Category-specific repository interface with specialized operations for categories
namespace BookManagementSystem.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Models.Category>
    {
        // Get a category with all books in that category
        Task<Models.Category> GetCategoryWithBooksAsync(int id);
    }
}