using System.ComponentModel.DataAnnotations; // For data annotations
using System.ComponentModel.DataAnnotations.Schema; // For Data Annotations such as [Phone] (if necessary)

namespace BookManagementSystem.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters.")]
        public required string Title { get; set; }  // Book title

        [Required(ErrorMessage = "ISBN is required.")]
        public required string ISBN { get; set; }  // ISBN number

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }  // Category Foreign Key

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(50, ErrorMessage = "Category Name can't be longer than 50 characters.")]
        public required string CategoryName { get; set; }  // Category name

        [Required(ErrorMessage = "Author is required.")]
        public int AuthorId { get; set; }  // Author Foreign Key

        [Required(ErrorMessage = "Author Name is required.")]
        [StringLength(50, ErrorMessage = "Author Name can't be longer than 50 characters.")]
        public required string AuthorName { get; set; }  // Author name

        [Required(ErrorMessage = "Publisher is required.")]
        public int PublisherId { get; set; }  // Publisher Foreign Key

        [Required(ErrorMessage = "Publisher Name is required.")]
        [StringLength(50, ErrorMessage = "Publisher Name can't be longer than 50 characters.")]
        public required string PublisherName { get; set; }  // Publisher name

        [Required(ErrorMessage = "Publisher Address is required.")]
        [StringLength(100, ErrorMessage = "Publisher Address can't be longer than 100 characters.")]
        public required string PublisherAddress { get; set; }  // Publisher address

        [Required(ErrorMessage = "Publisher Contact Number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public required string PublisherContactNumber { get; set; }  // Publisher contact number
    }
}
