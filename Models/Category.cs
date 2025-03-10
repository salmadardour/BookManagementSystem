using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BookManagementSystem.Models
{
    /// <summary>
    /// Represents a book category in the Book Management System
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique identifier for the category
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the category
        /// </summary>
        /// <example>Fantasy</example>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Collection of books in this category
        /// </summary>
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}