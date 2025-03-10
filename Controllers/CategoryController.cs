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
    /// Manages operations related to book categories in the Book Management System
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Initializes a new instance of the CategoryController
        /// </summary>
        /// <param name="context">Database context for category operations</param>
        /// <param name="logger">Logger for the controller</param>
        public CategoryController(BookManagementContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all book categories in the system
        /// </summary>
        /// <returns>A list of all categories</returns>
        /// <response code="200">Returns the list of categories</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            _logger.LogInformation("Fetching all categories.");
            try
            {
                var categories = await _context.Categories.ToListAsync();
                _logger.LogInformation("Fetched {CategoryCount} categories successfully.", categories.Count);

                var categoryDTOs = categories.Select(category => new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name
                }).ToList();

                return categoryDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching categories.");
            }
        }

        /// <summary>
        /// Gets a specific category by ID
        /// </summary>
        /// <param name="id">The ID of the category to retrieve</param>
        /// <returns>The requested category</returns>
        /// <response code="200">Returns the requested category</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            _logger.LogInformation("Fetching category with ID {CategoryId}.", id);
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Fetched category with ID {CategoryId} successfully.", id);

                var categoryDTO = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return categoryDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category with ID {CategoryId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching category.");
            }
        }

        /// <summary>
        /// Updates an existing category
        /// </summary>
        /// <param name="id">The ID of the category to update</param>
        /// <param name="categoryDTO">The updated category data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the category was updated successfully</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the request body</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutCategory(int id, CategoryDTO categoryDTO)
        {
            if (id != categoryDTO.Id)
            {
                _logger.LogWarning("Category ID mismatch: expected {ExpectedId}, got {ActualId}.", id, categoryDTO.Id);
                return BadRequest();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for update.", id);
                return NotFound();
            }

            category.Name = categoryDTO.Name;

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated category with ID {CategoryId} successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error updating category with ID {CategoryId}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="categoryDTO">The category data to create</param>
        /// <returns>The newly created category</returns>
        /// <response code="201">Returns the newly created category</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDTO>> PostCategory(CategoryDTO categoryDTO)
        {
            _logger.LogInformation("Creating a new category.");
            try
            {
                var category = new Category
                {
                    Name = categoryDTO.Name
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created category with ID {CategoryId} successfully.", category.Id);

                var createdCategoryDTO = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return CreatedAtAction("GetCategory", new { id = category.Id }, createdCategoryDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new category.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new category.");
            }
        }

        /// <summary>
        /// Deletes a specific category
        /// </summary>
        /// <param name="id">The ID of the category to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the category was deleted successfully</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("Deleting category with ID {CategoryId}.", id);
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found for deletion.", id);
                    return NotFound();
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted category with ID {CategoryId} successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID {CategoryId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting category.");
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}