// Defines operations available for reviews in the business logic layer
using BookManagementSystem.Models;

namespace BookManagementSystem.Services.Interfaces
{
    public interface IReviewService
    {
        // Get all reviews
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        
        // Get a specific review by ID
        Task<Review> GetReviewByIdAsync(int id);
        
        // Get all reviews for a specific book
        Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId);
        
        // Get average rating for a specific book
        Task<double> GetAverageRatingForBookAsync(int bookId);
        
        // Add a new review
        Task AddReviewAsync(Review review);
        
        // Update an existing review
        Task UpdateReviewAsync(Review review);
        
        // Delete a review
        Task DeleteReviewAsync(int id);
    }
}