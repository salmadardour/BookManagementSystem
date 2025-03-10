using System.ComponentModel.DataAnnotations; // Add this for data annotations

namespace BookManagementSystem.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Reviewer name is required.")]
        [StringLength(100, ErrorMessage = "Reviewer's name can't be longer than 100 characters.")]
        public required string ReviewerName { get; set; }  // Name of the Reviewer

        [Required(ErrorMessage = "Review content is required.")]
        public required string Content { get; set; }  // Review Content

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }  // Rating (e.g., 1 to 5)

        [Required(ErrorMessage = "Book ID is required.")]
        public int BookId { get; set; }  // Foreign Key to Book

        [Required(ErrorMessage = "Book title is required.")]
        public required string BookTitle { get; set; }  // Title of the book being reviewed
    }
}
