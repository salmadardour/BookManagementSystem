// Author-specific repository implementation
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;

namespace BookManagementSystem.Repositories
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        // Constructor passes the context to the base Repository class
        public AuthorRepository(BookManagementContext context) : base(context) { }

        // Get an author with all their books
        public async Task<Author> GetAuthorWithBooksAsync(int id)
        {
            return await _dbSet
                .Where(a => a.Id == id)
                .Include(a => a.Books)
                    .ThenInclude(b => b.Category)
                .Include(a => a.Books)
                    .ThenInclude(b => b.Publisher)
                .FirstOrDefaultAsync();
        }
    }
}