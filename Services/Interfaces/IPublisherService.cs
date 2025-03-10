// Defines operations available for publishers in the business logic layer
using BookManagementSystem.Models;

namespace BookManagementSystem.Services.Interfaces
{
    public interface IPublisherService
    {
        // Get all publishers
        Task<IEnumerable<Publisher>> GetAllPublishersAsync();
        
        // Get a specific publisher by ID
        Task<Publisher> GetPublisherByIdAsync(int id);
        
        // Get a publisher with all their published books
        Task<Publisher> GetPublisherWithBooksAsync(int id);
        
        // Add a new publisher
        Task AddPublisherAsync(Publisher publisher);
        
        // Update an existing publisher
        Task UpdatePublisherAsync(Publisher publisher);
        
        // Delete a publisher
        Task DeletePublisherAsync(int id);
    }
}