using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Identity;  // Add the namespace for ApplicationUser
using BookManagementSystem.Models;
using System.Collections.Generic;

namespace BookManagementSystem.Data
{
    public class BookManagementContext : IdentityDbContext<ApplicationUser>
    {
        public BookManagementContext(DbContextOptions<BookManagementContext> options)
            : base(options)
        { }

        // DbSets for each model
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Seeding method
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder); // Call base to apply Identity model schema

    // Configure relationships here.  Fluent API is used in this method.
    // One-to-many relationship between Author and Book
    modelBuilder.Entity<Book>()
        .HasOne(b => b.Author)
        .WithMany(a => a.Books)
        .HasForeignKey(b => b.AuthorId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

    // One-to-many relationship between Publisher and Book
    modelBuilder.Entity<Book>()
        .HasOne(b => b.Publisher)
        .WithMany(p => p.Books)
        .HasForeignKey(b => b.PublisherId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

    // One-to-many relationship between Category and Book
    modelBuilder.Entity<Book>()
        .HasOne(b => b.Category)
        .WithMany(c => c.Books)
        .HasForeignKey(b => b.CategoryId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

    // One-to-many relationship between Book and Review
    modelBuilder.Entity<Review>()
        .HasOne(r => r.Book)
        .WithMany() // Removed the Reviews Navigation property in Book Model, so no reference.
        .HasForeignKey(r => r.BookId)
        .OnDelete(DeleteBehavior.Cascade); // If book deleted, delete reviews

    //Seeding Authors
    modelBuilder.Entity<Author>().HasData(
        new Author { Id = 1, Name = "J.K. Rowling" },
        new Author { Id = 2, Name = "Isaac Newton" }
    );

    //Seeding Categories
    modelBuilder.Entity<Category>().HasData(
        new Category { Id = 1, Name = "Fantasy" },
        new Category { Id = 2, Name = "Science" }
    );

    //Seeding Publishers
    modelBuilder.Entity<Publisher>().HasData(
        new Publisher { Id = 1, Name = "Bloomsbury", Address = "London", ContactNumber = "123456789" },
        new Publisher { Id = 2, Name = "Cambridge", Address = "Cambridge", ContactNumber = "987654321" }
    );

    //Seeding Books
    modelBuilder.Entity<Book>().HasData(
        new Book { Id = 1, Title = "Harry Potter and the Sorcerer's Stone", ISBN = "123-456789", CategoryId = 1, AuthorId = 1, PublisherId = 1 },
        new Book { Id = 2, Title = "Philosophiæ Naturalis Principia Mathematica", ISBN = "987-654321", CategoryId = 2, AuthorId = 2, PublisherId = 2 }
    );

    //Seeding Review
    modelBuilder.Entity<Review>().HasData(
        new Review { Id = 1, ReviewerName = "John Doe", Content = "Amazing book!", Rating = 5, BookId = 1 },
        new Review { Id = 2, ReviewerName = "Jane Smith", Content = "Very insightful.", Rating = 5, BookId = 2 }
    );
}

    }
}
