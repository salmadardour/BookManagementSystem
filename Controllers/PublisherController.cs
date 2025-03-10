using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.DTO; // Ensure DTO is correctly imported
using Microsoft.Extensions.Logging; // For ILogger

namespace BookManagementSystem.Controllers
{
    /// <summary>
    /// Manages operations related to publishers in the Book Management System
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<PublisherController> _logger;

        /// <summary>
        /// Initializes a new instance of the PublisherController
        /// </summary>
        /// <param name="context">Database context for publisher operations</param>
        /// <param name="logger">Logger for the controller</param>
        public PublisherController(BookManagementContext context, ILogger<PublisherController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all publishers in the system
        /// </summary>
        /// <returns>A list of all publishers</returns>
        /// <response code="200">Returns the list of publishers</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PublisherDTO>>> GetPublishers()
        {
            _logger.LogInformation("Fetching all publishers.");
            try
            {
                var publishers = await _context.Publishers.ToListAsync();
                _logger.LogInformation("Fetched {PublisherCount} publishers successfully.", publishers.Count);

                var publisherDTOs = publishers.Select(publisher => new PublisherDTO
                {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Address = publisher.Address,
                    ContactNumber = publisher.ContactNumber
                }).ToList();

                return publisherDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching publishers.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching publishers.");
            }
        }

        /// <summary>
        /// Gets a specific publisher by ID
        /// </summary>
        /// <param name="id">The ID of the publisher to retrieve</param>
        /// <returns>The requested publisher</returns>
        /// <response code="200">Returns the requested publisher</response>
        /// <response code="404">If the publisher is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublisherDTO>> GetPublisher(int id)
        {
            _logger.LogInformation("Fetching publisher with ID {PublisherId}.", id);
            try
            {
                var publisher = await _context.Publishers.FindAsync(id);

                if (publisher == null)
                {
                    _logger.LogWarning("Publisher with ID {PublisherId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched publisher with ID {PublisherId} successfully.", id);

                var publisherDTO = new PublisherDTO
                {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Address = publisher.Address,
                    ContactNumber = publisher.ContactNumber
                };

                return publisherDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching publisher with ID {PublisherId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching publisher.");
            }
        }

        /// <summary>
        /// Updates an existing publisher
        /// </summary>
        /// <param name="id">The ID of the publisher to update</param>
        /// <param name="publisherDTO">The updated publisher data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the publisher was updated successfully</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the request body</response>
        /// <response code="404">If the publisher is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPublisher(int id, PublisherDTO publisherDTO)
        {
            if (id != publisherDTO.Id)
            {
                _logger.LogWarning("Publisher ID mismatch: expected {ExpectedId}, got {ActualId}.", id, publisherDTO.Id);
                return BadRequest();
            }

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                _logger.LogWarning("Publisher with ID {PublisherId} not found for update.", id);
                return NotFound();
            }

            publisher.Name = publisherDTO.Name;
            publisher.Address = publisherDTO.Address;
            publisher.ContactNumber = publisherDTO.ContactNumber;

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated publisher with ID {PublisherId} successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
                {
                    _logger.LogWarning("Publisher with ID {PublisherId} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error updating publisher with ID {PublisherId}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new publisher
        /// </summary>
        /// <param name="publisherDTO">The publisher data to create</param>
        /// <returns>The newly created publisher</returns>
        /// <response code="201">Returns the newly created publisher</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublisherDTO>> PostPublisher(PublisherDTO publisherDTO)
        {
            _logger.LogInformation("Creating a new publisher.");
            try
            {
                var publisher = new Publisher
                {
                    Name = publisherDTO.Name,
                    Address = publisherDTO.Address,
                    ContactNumber = publisherDTO.ContactNumber
                };

                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created publisher with ID {PublisherId} successfully.", publisher.Id);

                var createdPublisherDTO = new PublisherDTO
                {
                    Id = publisher.Id,
                    Name = publisher.Name,
                    Address = publisher.Address,
                    ContactNumber = publisher.ContactNumber
                };

                return CreatedAtAction("GetPublisher", new { id = publisher.Id }, createdPublisherDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new publisher.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new publisher.");
            }
        }

        /// <summary>
        /// Deletes a specific publisher
        /// </summary>
        /// <param name="id">The ID of the publisher to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the publisher was deleted successfully</response>
        /// <response code="404">If the publisher is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            _logger.LogInformation("Deleting publisher with ID {PublisherId}.", id);
            try
            {
                var publisher = await _context.Publishers.FindAsync(id);
                if (publisher == null)
                {
                    _logger.LogWarning("Publisher with ID {PublisherId} not found for deletion.", id);
                    return NotFound();
                }

                _context.Publishers.Remove(publisher);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted publisher with ID {PublisherId} successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting publisher with ID {PublisherId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting publisher.");
            }
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}