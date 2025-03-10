using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BookManagementSystem.Models
{
    /// <summary>
    /// Represents an author in the Book Management System
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Unique identifier for the author
        /// </summary>
        /// <example>1</example>
        [Key] //Marks Id as Primary Key
        public int Id { get; set; }

        /// <summary>
        /// Name of the author
        /// </summary>
        /// <example>J.K. Rowling</example>
        [Required(ErrorMessage = "Name is required")] //Validation: Ensures the Name is not null or empty
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")] //Validation: Limits the length of the Name
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Collection of books written by this author
        /// </summary>
        public ICollection<Book> Books { get; set; } = new List<Book>(); // Navigation Property: Represents the relationship with Books
    }
}