using System.ComponentModel.DataAnnotations; // For data annotations

namespace BookManagementSystem.DTO
{
    public class PublisherDTO
    {
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Publisher name is required.")]
        [StringLength(100, ErrorMessage = "Publisher's name can't be longer than 100 characters.")]
        public required string Name { get; set; }  // Publisher's name

        [Required(ErrorMessage = "Publisher address is required.")]
        [StringLength(200, ErrorMessage = "Publisher's address can't be longer than 200 characters.")]
        public required string Address { get; set; }  // Publisher's address

        [Required(ErrorMessage = "Publisher contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public required string ContactNumber { get; set; }  // Publisher's contact number
    }
}
