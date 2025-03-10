// Models/Review.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookManagementSystem.Models
{
    /// <summary>
    /// Represents a book review in the Book Management System
    /// </summary>
    public class Review
    {
        /// <summary>
        /// Unique identifier for the review
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the person who provided the review
        /// </summary>
        /// <example>John Doe</example>
        [Required(ErrorMessage = "Reviewer Name is required")]
        [StringLength(100, ErrorMessage = "Reviewer Name cannot exceed 100 characters")]
        public string ReviewerName { get; set; } = string.Empty;

        /// <summary>
        /// Text content of the review
        /// </summary>
        /// <example>This book was a fantastic read with engaging characters and an exciting plot.</example>
        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Rating given to the book (1-5 stars)
        /// </summary>
        /// <example>5</example>
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]  //Validation for Rating
        public int Rating { get; set; }

        /// <summary>
        /// ID of the book being reviewed
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Book is required")] //Makes Book a required field
        public int BookId { get; set; }

        /// <summary>
        /// Book being reviewed
        /// </summary>
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}