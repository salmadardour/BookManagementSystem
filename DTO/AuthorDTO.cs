using System.ComponentModel.DataAnnotations;  // For data annotations

namespace BookManagementSystem.DTO
{
    public class AuthorDTO
    {
        public int Id { get; set; }  // Primary Key

        [Required]  // Makes this field required
        [StringLength(100)]  // Sets a max length for the name
        public required string Name { get; set; }  // Author's name
    }
}
