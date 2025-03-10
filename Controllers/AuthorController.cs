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
    /// Manages operations related to authors in the Book Management System
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<AuthorController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthorController
        /// </summary>
        /// <param name="context">Database context for author operations</param>
        /// <param name="logger">Logger for the controller</param>
        public AuthorController(BookManagementContext context, ILogger<AuthorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all authors in the system
        /// </summary>
        /// <returns>A list of all authors</returns>
        /// <response code="200">Returns the list of authors</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            _logger.LogInformation("Fetching all authors.");
            try
            {
                var authors = await _context.Authors.ToListAsync();
                _logger.LogInformation("Fetched {AuthorCount} authors successfully.", authors.Count);

                var authorDTOs = authors.Select(author => new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name
                }).ToList();

                return authorDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching authors.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching authors.");
            }
        }

        /// <summary>
        /// Gets a specific author by ID
        /// </summary>
        /// <param name="id">The ID of the author to retrieve</param>
        /// <returns>The requested author</returns>
        /// <response code="200">Returns the requested author</response>
        /// <response code="404">If the author is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            _logger.LogInformation("Fetching author with ID {AuthorId}.", id);
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched author with ID {AuthorId} successfully.", id);

                var authorDTO = new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name
                };

                return authorDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching author with ID {AuthorId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching author.");
            }
        }

        /// <summary>
        /// Updates an existing author
        /// </summary>
        /// <param name="id">The ID of the author to update</param>
        /// <param name="authorDTO">The updated author data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the author was updated successfully</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the request body</response>
        /// <response code="404">If the author is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAuthor(int id, AuthorDTO authorDTO)
        {
            if (id != authorDTO.Id)
            {
                _logger.LogWarning("Author ID mismatch: expected {ExpectedId}, got {ActualId}.", id, authorDTO.Id);
                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                _logger.LogWarning("Author with ID {AuthorId} not found for update.", id);
                return NotFound();
            }

            author.Name = authorDTO.Name;

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated author with ID {AuthorId} successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error updating author with ID {AuthorId}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new author
        /// </summary>
        /// <param name="authorDTO">The author data to create</param>
        /// <returns>The newly created author</returns>
        /// <response code="201">Returns the newly created author</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> PostAuthor(AuthorDTO authorDTO)
        {
            _logger.LogInformation("Creating a new author.");
            try
            {
                var author = new Author
                {
                    Name = authorDTO.Name
                };

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created author with ID {AuthorId} successfully.", author.Id);

                var createdAuthorDTO = new AuthorDTO
                {
                    Id = author.Id,
                    Name = author.Name
                };

                return CreatedAtAction("GetAuthor", new { id = author.Id }, createdAuthorDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new author.");
            }
        }

        /// <summary>
        /// Deletes a specific author
        /// </summary>
        /// <param name="id">The ID of the author to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the author was deleted successfully</response>
        /// <response code="404">If the author is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            _logger.LogInformation("Deleting author with ID {AuthorId}.", id);
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    _logger.LogWarning("Author with ID {AuthorId} not found for deletion.", id);
                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted author with ID {AuthorId} successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting author with ID {AuthorId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting author.");
            }
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}