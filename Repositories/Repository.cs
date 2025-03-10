// Generic repository implementation that handles basic CRUD operations for any entity
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Data;
using BookManagementSystem.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BookManagementSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // Database context and entity set
        protected readonly BookManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        // Constructor injects the database context
        public Repository(BookManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Get all entities of type T
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Get entity by its ID
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Find entities based on a predicate (condition)
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Add a new entity
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Update an existing entity
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        // Delete an entity
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}