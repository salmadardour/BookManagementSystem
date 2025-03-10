// Implements business logic for publisher operations
using BookManagementSystem.Models;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookManagementSystem.Services
{
    public class PublisherService : IPublisherService
    {
        // Dependencies injected through constructor
        private readonly IPublisherRepository _publisherRepository;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(IPublisherRepository publisherRepository, ILogger<PublisherService> logger)
        {
            _publisherRepository = publisherRepository;
            _logger = logger;
        }

        // Get all publishers
        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            try
            {
                _logger.LogInformation("Getting all publishers");
                return await _publisherRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all publishers");
                throw;
            }
        }

        // Get a specific publisher by ID
        public async Task<Publisher> GetPublisherByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting publisher with ID: {id}");
                var publisher = await _publisherRepository.GetByIdAsync(id);
                
                if (publisher == null)
                {
                    _logger.LogWarning($"Publisher with ID: {id} was not found");
                }
                
                return publisher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving publisher with ID: {id}");
                throw;
            }
        }

        // Get a publisher with all books published by them
        public async Task<Publisher> GetPublisherWithBooksAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting publisher with books for ID: {id}");
                var publisher = await _publisherRepository.GetPublisherWithBooksAsync(id);
                
                if (publisher == null)
                {
                    _logger.LogWarning($"Publisher with ID: {id} was not found");
                }
                
                return publisher;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving publisher with books for ID: {id}");
                throw;
            }
        }

        // Add a new publisher
        public async Task AddPublisherAsync(Publisher publisher)
        {
            try
            {
                _logger.LogInformation($"Adding new publisher: {publisher.Name}");
                await _publisherRepository.AddAsync(publisher);
                await _publisherRepository.SaveChangesAsync();
                _logger.LogInformation($"Publisher added successfully with ID: {publisher.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding publisher: {publisher.Name}");
                throw;
            }
        }

        // Update an existing publisher
        public async Task UpdatePublisherAsync(Publisher publisher)
        {
            try
            {
                _logger.LogInformation($"Updating publisher with ID: {publisher.Id}");
                await _publisherRepository.UpdateAsync(publisher);
                await _publisherRepository.SaveChangesAsync();
                _logger.LogInformation($"Publisher updated successfully with ID: {publisher.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating publisher with ID: {publisher.Id}");
                throw;
            }
        }

        // Delete a publisher
        public async Task DeletePublisherAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting publisher with ID: {id}");
                var publisher = await _publisherRepository.GetByIdAsync(id);
                
                if (publisher == null)
                {
                    _logger.LogWarning($"Publisher with ID: {id} not found for deletion");
                    throw new ArgumentException($"Publisher with ID: {id} not found");
                }
                
                await _publisherRepository.DeleteAsync(publisher);
                await _publisherRepository.SaveChangesAsync();
                _logger.LogInformation($"Publisher deleted successfully with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting publisher with ID: {id}");
                throw;
            }
        }
    }
}