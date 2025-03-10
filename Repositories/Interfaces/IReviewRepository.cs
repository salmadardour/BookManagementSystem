// Review-specific repository interface with specialized operations for reviews
namespace BookManagementSystem.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Models.Review>
    {
        // Get all reviews for a specific book
        Task<IEnumerable<Models.Review>> GetReviewsByBookAsync(int bookId);
        
        // Get average rating for a specific book
        Task<double> GetAverageRatingForBookAsync(int bookId);
    }
}