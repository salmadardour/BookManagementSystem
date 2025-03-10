// Implements business logic for review operations
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagementSystem.Services
{
    public class ReviewService : IReviewService
    {
        // Dependencies injected through constructor
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository, IBookRepository bookRepository, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        // Get all reviews
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all reviews");
                return await _reviewRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reviews");
                throw;
            }
        }

        // Get a specific review by ID
        public async Task<Review> GetReviewByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting review with ID: {id}");
                var review = await _reviewRepository.GetByIdAsync(id);
                
                if (review == null)
                {
                    _logger.LogWarning($"Review with ID: {id} was not found");
                }
                
                return review;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving review with ID: {id}");
                throw;
            }
        }

        // Get all reviews for a specific book
        public async Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId)
        {
            try
            {
                _logger.LogInformation($"Getting reviews for book ID: {bookId}");
                
                // Verify book exists first
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {bookId} not found when retrieving reviews");
                    throw new ArgumentException($"Book with ID: {bookId} not found");
                }
                
                return await _reviewRepository.GetReviewsByBookAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving reviews for book ID: {bookId}");
                throw;
            }
        }

        // Get the average rating for a specific book
        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            try
            {
                _logger.LogInformation($"Getting average rating for book ID: {bookId}");
                
                // Verify book exists first
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {bookId} not found when retrieving average rating");
                    throw new ArgumentException($"Book with ID: {bookId} not found");
                }
                
                return await _reviewRepository.GetAverageRatingForBookAsync(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving average rating for book ID: {bookId}");
                throw;
            }
        }

        // Add a new review
        public async Task AddReviewAsync(Review review)
        {
            try
            {
                _logger.LogInformation($"Adding new review for book ID: {review.BookId}");
                
                // Verify book exists first
                var book = await _bookRepository.GetByIdAsync(review.BookId);
                if (book == null)
                {
                    _logger.LogWarning($"Book with ID: {review.BookId} not found when adding review");
                    throw new ArgumentException($"Book with ID: {review.BookId} not found");
                }
                
                // Validate rating is within acceptable range (1-5)
                if (review.Rating < 1 || review.Rating > 5)
                {
                    _logger.LogWarning($"Invalid rating value: {review.Rating}. Rating must be between 1 and 5");
                    throw new ArgumentException("Rating must be between 1 and 5");
                }
                
                await _reviewRepository.AddAsync(review);
                await _reviewRepository.SaveChangesAsync();
                _logger.LogInformation($"Review added successfully with ID: {review.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding review for book ID: {review.BookId}");
                throw;
            }
        }

        // Update an existing review
        public async Task UpdateReviewAsync(Review review)
        {
            try
            {
                _logger.LogInformation($"Updating review with ID: {review.Id}");
                
                // Validate rating is within acceptable range (1-5)
                if (review.Rating < 1 || review.Rating > 5)
                {
                    _logger.LogWarning($"Invalid rating value: {review.Rating}. Rating must be between 1 and 5");
                    throw new ArgumentException("Rating must be between 1 and 5");
                }
                
                await _reviewRepository.UpdateAsync(review);
                await _reviewRepository.SaveChangesAsync();
                _logger.LogInformation($"Review updated successfully with ID: {review.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating review with ID: {review.Id}");
                throw;
            }
        }

        // Delete a review
        public async Task DeleteReviewAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting review with ID: {id}");
                var review = await _reviewRepository.GetByIdAsync(id);
                
                if (review == null)
                {
                    _logger.LogWarning($"Review with ID: {id} not found for deletion");
                    throw new ArgumentException($"Review with ID: {id} not found");
                }
                
                await _reviewRepository.DeleteAsync(review);
                await _reviewRepository.SaveChangesAsync();
                _logger.LogInformation($"Review deleted successfully with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting review with ID: {id}");
                throw;
            }
        }
    }
}