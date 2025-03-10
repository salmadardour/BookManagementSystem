// Book-specific repository implementation
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;

namespace BookManagementSystem.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        // Constructor passes the context to the base Repository class
        public BookRepository(BookManagementContext context) : base(context) { }

        // Get all books by a specific author, including related entities
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _dbSet
                .Where(b => b.AuthorId == authorId)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .ToListAsync();
        }

        // Get all books in a specific category, including related entities
        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(b => b.CategoryId == categoryId)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .ToListAsync();
        }

        // Get all books from a specific publisher, including related entities
        public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId)
        {
            return await _dbSet
                .Where(b => b.PublisherId == publisherId)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .ToListAsync();
        }

        // Get a book with all its related entities (author, category, publisher)
        public async Task<Book> GetBookWithDetailsAsync(int id)
        {
            return await _dbSet
                .Where(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync();
        }
    }
}