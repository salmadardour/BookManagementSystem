using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Required for [ForeignKey]

namespace BookManagementSystem.Models
{
    /// <summary>
    /// Represents a book in the Book Management System
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Unique identifier for the book
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Title of the book
        /// </summary>
        /// <example>Harry Potter and the Sorcerer's Stone</example>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// International Standard Book Number (ISBN) of the book
        /// </summary>
        /// <example>123-456789</example>
        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")] // Adjust length as needed
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// ID of the category the book belongs to
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Category is required")] //Makes Category a required field
        public int CategoryId { get; set; }

        /// <summary>
        /// Category the book belongs to
        /// </summary>
        [ForeignKey("CategoryId")] //Specifies that CategoryId is a Foreign Key to Category
        public Category? Category { get; set; }

        /// <summary>
        /// ID of the author of the book
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Author is required")] //Makes Author a required field
        public int AuthorId { get; set; }

        /// <summary>
        /// Author of the book
        /// </summary>
        [ForeignKey("AuthorId")] //Specifies that AuthorId is a Foreign Key to Author
        public Author? Author { get; set; }

        /// <summary>
        /// ID of the publisher of the book
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "Publisher is required")] //Makes Publisher a required field
        public int PublisherId { get; set; }

        /// <summary>
        /// Publisher of the book
        /// </summary>
        [ForeignKey("PublisherId")] //Specifies that PublisherId is a Foreign Key to Publisher
        public Publisher? Publisher { get; set; }
    }
}