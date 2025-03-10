namespace BookManagementSystem.DTO
{
    public class RegisterDTO
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public string Role { get; set; } = "User"; // Default role is "User"
    }
}
