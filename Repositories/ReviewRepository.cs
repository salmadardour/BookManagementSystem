// Review-specific repository implementation
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;

namespace BookManagementSystem.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        // Constructor passes the context to the base Repository class
        public ReviewRepository(BookManagementContext context) : base(context) { }

        // Get all reviews for a specific book
        public async Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId)
        {
            return await _dbSet
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }

        // Get average rating for a specific book
        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            var reviews = await _dbSet
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (reviews == null || !reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}