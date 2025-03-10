using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.DTO; // Ensure DTO is correctly imported
using Microsoft.Extensions.Logging; // For ILogger

namespace BookManagementSystem.Controllers
{
    /// <summary>
    /// Manages operations related to book reviews in the Book Management System
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<ReviewController> _logger;

        /// <summary>
        /// Initializes a new instance of the ReviewController
        /// </summary>
        /// <param name="context">Database context for review operations</param>
        /// <param name="logger">Logger for the controller</param>
        public ReviewController(BookManagementContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all book reviews in the system
        /// </summary>
        /// <returns>A list of all reviews with book information</returns>
        /// <response code="200">Returns the list of reviews</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
        {
            _logger.LogInformation("Fetching all reviews.");
            try
            {
                var reviews = await _context.Reviews
                    .Include(static r => r.Book) // Including related Book data for DTO conversion
                    .ToListAsync();

                _logger.LogInformation("Fetched {ReviewCount} reviews successfully.", reviews.Count);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var reviewDTOs = reviews.Select(static review => new ReviewDTO
                {
                    Id = review.Id,
                    ReviewerName = review.ReviewerName,
                    Content = review.Content,
                    Rating = review.Rating,
                    BookId = review.BookId,
                    BookTitle = review.Book.Title // Using Book's title as part of the DTO
                }).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return reviewDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching reviews.");
            }
        }

        /// <summary>
        /// Gets a specific review by ID
        /// </summary>
        /// <param name="id">The ID of the review to retrieve</param>
        /// <returns>The requested review with book information</returns>
        /// <response code="200">Returns the requested review</response>
        /// <response code="404">If the review is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            _logger.LogInformation("Fetching review with ID {ReviewId}.", id);
            try
            {
                var review = await _context.Reviews
                    .Include(r => r.Book) // Including related Book data for DTO conversion
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null)
                {
                    _logger.LogWarning("Review with ID {ReviewId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched review with ID {ReviewId} successfully.", id);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var reviewDTO = new ReviewDTO
                {
                    Id = review.Id,
                    ReviewerName = review.ReviewerName,
                    Content = review.Content,
                    Rating = review.Rating,
                    BookId = review.BookId,
                    BookTitle = review.Book.Title // Using Book's title as part of the DTO
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return reviewDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review with ID {ReviewId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching review.");
            }
        }

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="id">The ID of the review to update</param>
        /// <param name="reviewDTO">The updated review data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the review was updated successfully</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the request body</response>
        /// <response code="404">If the review is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutReview(int id, ReviewDTO reviewDTO)
        {
            if (id != reviewDTO.Id)
            {
                _logger.LogWarning("Review ID mismatch: expected {ExpectedId}, got {ActualId}.", id, reviewDTO.Id);
                return BadRequest();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review with ID {ReviewId} not found for update.", id);
                return NotFound();
            }

            review.ReviewerName = reviewDTO.ReviewerName;
            review.Content = reviewDTO.Content;
            review.Rating = reviewDTO.Rating;
            review.BookId = reviewDTO.BookId;

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated review with ID {ReviewId} successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    _logger.LogWarning("Review with ID {ReviewId} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error updating review with ID {ReviewId}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new review for a book
        /// </summary>
        /// <param name="reviewDTO">The review data to create</param>
        /// <returns>The newly created review</returns>
        /// <response code="201">Returns the newly created review</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ReviewDTO>> PostReview(ReviewDTO reviewDTO)
        {
            _logger.LogInformation("Creating a new review.");
            try
            {
                var review = new Review
                {
                    ReviewerName = reviewDTO.ReviewerName,
                    Content = reviewDTO.Content,
                    Rating = reviewDTO.Rating,
                    BookId = reviewDTO.BookId
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created review with ID {ReviewId} successfully.", review.Id);

                // Load the book to get the title for the response
                await _context.Entry(review).Reference(r => r.Book).LoadAsync();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var createdReviewDTO = new ReviewDTO
                {
                    Id = review.Id,
                    ReviewerName = review.ReviewerName,
                    Content = review.Content,
                    Rating = review.Rating,
                    BookId = review.BookId,
                    BookTitle = review.Book.Title // Adding Book title to DTO
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return CreatedAtAction("GetReview", new { id = review.Id }, createdReviewDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new review.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new review.");
            }
        }

        /// <summary>
        /// Deletes a specific review
        /// </summary>
        /// <param name="id">The ID of the review to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the review was deleted successfully</response>
        /// <response code="404">If the review is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            _logger.LogInformation("Deleting review with ID {ReviewId}.", id);
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID {ReviewId} not found for deletion.", id);
                    return NotFound();
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted review with ID {ReviewId} successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID {ReviewId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting review.");
            }
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }
    }
}