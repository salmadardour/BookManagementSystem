using Microsoft.AspNetCore.Identity;
using System;

namespace BookManagementSystem.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // ✅ Allow NULL values to prevent NOT NULL constraint errors
        public string? RefreshToken { get; set; } = null;
        public DateTime? RefreshTokenExpiryTime { get; set; } = null;
    }
}
