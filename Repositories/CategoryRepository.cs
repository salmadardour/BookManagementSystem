// Category-specific repository implementation
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;

namespace BookManagementSystem.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        // Constructor passes the context to the base Repository class
        public CategoryRepository(BookManagementContext context) : base(context) { }

        // Get a category with all books in that category
        public async Task<Category> GetCategoryWithBooksAsync(int id)
        {
            return await _dbSet
                .Where(c => c.Id == id)
                .Include(c => c.Books)
                    .ThenInclude(b => b.Author)
                .Include(c => c.Books)
                    .ThenInclude(b => b.Publisher)
                .FirstOrDefaultAsync();
        }
    }
}