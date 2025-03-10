// Publisher-specific repository implementation
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;

namespace BookManagementSystem.Repositories
{
    public class PublisherRepository : Repository<Publisher>, IPublisherRepository
    {
        // Constructor passes the context to the base Repository class
        public PublisherRepository(BookManagementContext context) : base(context) { }

        // Get a publisher with all their published books
        public async Task<Publisher> GetPublisherWithBooksAsync(int id)
        {
            return await _dbSet
                .Where(p => p.Id == id)
                .Include(p => p.Books)
                    .ThenInclude(b => b.Author)
                .Include(p => p.Books)
                    .ThenInclude(b => b.Category)
                .FirstOrDefaultAsync();
        }
    }
}
