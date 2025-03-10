// Defines operations available for categories in the business logic layer
using BookManagementSystem.Models;

namespace BookManagementSystem.Services.Interfaces
{
    public interface ICategoryService
    {
        // Get all categories
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        
        // Get a specific category by ID
        Task<Category> GetCategoryByIdAsync(int id);
        
        // Get a category with all books in that category
        Task<Category> GetCategoryWithBooksAsync(int id);
        
        // Add a new category
        Task AddCategoryAsync(Category category);
        
        // Update an existing category
        Task UpdateCategoryAsync(Category category);
        
        // Delete a category
        Task DeleteCategoryAsync(int id);
    }
}