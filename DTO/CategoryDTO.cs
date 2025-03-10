using System.ComponentModel.DataAnnotations; // For data annotations

namespace BookManagementSystem.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, ErrorMessage = "Category name can't be longer than 50 characters.")]
        public required string Name { get; set; }  // Category name
    }
}
