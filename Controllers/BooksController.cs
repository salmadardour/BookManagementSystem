using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Models;
using BookManagementSystem.DTO;  // Ensure DTO is correctly imported
using Microsoft.Extensions.Logging;
using BookManagementSystem.Services.Interfaces; // For ILogger

namespace BookManagementSystem.Controllers
{
    /// <summary>
    /// Manages operations related to books in the Book Management System
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookManagementContext _context;
        private readonly ILogger<BooksController> _logger;

        /// <summary>
        /// Initializes a new instance of the BooksController
        /// </summary>
        /// <param name="context">Database context for book operations</param>
        /// <param name="logger">Logger for the controller</param>
        public BooksController(BookManagementContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public BooksController(IBookService object1, ILogger<BooksController> object2)
        {
        }

        /// <summary>
        /// Gets all books with their associated author, category and publisher details
        /// </summary>
        /// <returns>A list of books with detailed information</returns>
        /// <response code="200">Returns the list of books</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            _logger.LogInformation("Fetching all books from the database.");
            try
            {
                var books = await _context.Books
                    .Include(static b => b.Author)
                    .Include(static b => b.Category)
                    .Include(static b => b.Publisher)
                    .ToListAsync();

                _logger.LogInformation($"Found {books.Count} books.");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var bookDTOs = books.Select(static book => new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    AuthorName = book.Author.Name,  // Using AuthorName
                    CategoryName = book.Category.Name,  // Using CategoryName
                    PublisherName = book.Publisher.Name,  // Using PublisherName
                    PublisherAddress = book.Publisher.Address,  // Using PublisherAddress
                    PublisherContactNumber = book.Publisher.ContactNumber  // Using PublisherContactNumber
                }).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return bookDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching books.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching books.");
            }
        }

        /// <summary>
        /// Gets a specific book by its ID with detailed information
        /// </summary>
        /// <param name="id">The ID of the book to retrieve</param>
        /// <returns>The requested book with details</returns>
        /// <response code="200">Returns the requested book</response>
        /// <response code="404">If the book is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            _logger.LogInformation($"Fetching book with id {id}.");
            try
            {
                var book = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Include(b => b.Publisher)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _logger.LogWarning($"Book with id {id} not found.");
                    return NotFound();
                }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var bookDTO = new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    AuthorName = book.Author.Name,
                    CategoryName = book.Category.Name,
                    PublisherName = book.Publisher.Name,
                    PublisherAddress = book.Publisher.Address,
                    PublisherContactNumber = book.Publisher.ContactNumber
                };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                return bookDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching book.");
            }
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="id">The ID of the book to update</param>
        /// <param name="bookDTO">The updated book data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was updated successfully</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the request body</response>
        /// <response code="404">If the book is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutBook(int id, BookDTO bookDTO)
        {
            _logger.LogInformation($"Updating book with id {id}.");
            if (id != bookDTO.Id)
            {
                _logger.LogWarning($"Book id {id} does not match the dto id.");
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                _logger.LogWarning($"Book with id {id} not found.");
                return NotFound();
            }

            book.Title = bookDTO.Title;
            book.ISBN = bookDTO.ISBN;
            book.CategoryId = bookDTO.CategoryId;  // Use CategoryId
            book.AuthorId = bookDTO.AuthorId;  // Use AuthorId
            book.PublisherId = bookDTO.PublisherId;  // Use PublisherId

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Book with id {id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    _logger.LogWarning($"Book with id {id} not found during update.");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Error updating book with id {id}.");
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="bookDTO">The book data to create</param>
        /// <returns>The newly created book</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> PostBook(BookDTO bookDTO)
        {
            _logger.LogInformation("Adding a new book.");
            try
            {
                var book = new Book
                {
                    Title = bookDTO.Title,
                    ISBN = bookDTO.ISBN,
                    CategoryId = bookDTO.CategoryId,
                    AuthorId = bookDTO.AuthorId,
                    PublisherId = bookDTO.PublisherId
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Book with id {book.Id} added successfully.");

                var createdBookDTO = new BookDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN = book.ISBN,
                    AuthorName = bookDTO.AuthorName,
                    CategoryName = bookDTO.CategoryName,
                    PublisherName = bookDTO.PublisherName,
                    PublisherAddress = bookDTO.PublisherAddress,  // Ensure PublisherAddress is set
                    PublisherContactNumber = bookDTO.PublisherContactNumber  // Ensure PublisherContactNumber is set
                };

                return CreatedAtAction("GetBook", new { id = book.Id }, createdBookDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new book.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding new book.");
            }
        }

        /// <summary>
        /// Deletes a specific book
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was deleted successfully</response>
        /// <response code="404">If the book is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation($"Deleting book with id {id}.");
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Book with id {id} not found for deletion.");
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Book with id {id} deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with id {BookId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting book.");
            }
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}