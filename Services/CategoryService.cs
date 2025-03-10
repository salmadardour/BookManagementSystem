// Implements business logic for category operations
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagementSystem.Services
{
    public class CategoryService : ICategoryService
    {
        // Dependencies injected through constructor
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        // Get all categories
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all categories");
                return await _categoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                throw;
            }
        }

        // Get a specific category by ID
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting category with ID: {id}");
                var category = await _categoryRepository.GetByIdAsync(id);
                
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} was not found");
                }
                
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category with ID: {id}");
                throw;
            }
        }

        // Get a category with all books in that category
        public async Task<Category> GetCategoryWithBooksAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting category with books for ID: {id}");
                var category = await _categoryRepository.GetCategoryWithBooksAsync(id);
                
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} was not found");
                }
                
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving category with books for ID: {id}");
                throw;
            }
        }

        // Add a new category
        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                _logger.LogInformation($"Adding new category: {category.Name}");
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();
                _logger.LogInformation($"Category added successfully with ID: {category.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding category: {category.Name}");
                throw;
            }
        }

        // Update an existing category
        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                _logger.LogInformation($"Updating category with ID: {category.Id}");
                await _categoryRepository.UpdateAsync(category);
                await _categoryRepository.SaveChangesAsync();
                _logger.LogInformation($"Category updated successfully with ID: {category.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category with ID: {category.Id}");
                throw;
            }
        }

        // Delete a category
        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting category with ID: {id}");
                var category = await _categoryRepository.GetByIdAsync(id);
                
                if (category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} not found for deletion");
                    throw new ArgumentException($"Category with ID: {id} not found");
                }
                
                await _categoryRepository.DeleteAsync(category);
                await _categoryRepository.SaveChangesAsync();
                _logger.LogInformation($"Category deleted successfully with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with ID: {id}");
                throw;
            }
        }
    }
}
