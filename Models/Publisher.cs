using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BookManagementSystem.Models
{
    /// <summary>
    /// Represents a publisher in the Book Management System
    /// </summary>
    public class Publisher
    {
        /// <summary>
        /// Unique identifier for the publisher
        /// </summary>
        /// <example>1</example>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the publisher
        /// </summary>
        /// <example>Bloomsbury</example>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Address of the publisher's headquarters or main office
        /// </summary>
        /// <example>50 Bedford Square, London, WC1B 3DP, UK</example>
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]  //Optional Field Validation
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Contact number for the publisher
        /// </summary>
        /// <example>+44 20 7631 5600</example>
        [StringLength(20, ErrorMessage = "Contact Number cannot exceed 20 characters")] //Optional Field Validation
        [RegularExpression(@"^(\+\d{1,3})?(\d{6,15})$", ErrorMessage = "Invalid Contact Number")] //Optional Field Validation
        public string ContactNumber { get; set; } = string.Empty;

        /// <summary>
        /// Collection of books published by this publisher
        /// </summary>
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}