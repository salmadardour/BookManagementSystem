// Generic repository interface that defines standard CRUD operations for any entity
using System.Linq.Expressions;

namespace BookManagementSystem.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Get all entities of type T
        Task<IEnumerable<T>> GetAllAsync();
        
        // Get entity by its ID
        Task<T> GetByIdAsync(int id);
        
        // Find entities based on a predicate (condition)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        // Add a new entity
        Task AddAsync(T entity);
        
        // Update an existing entity
        Task UpdateAsync(T entity);
        
        // Delete an entity
        Task DeleteAsync(T entity);
        
        // Save changes to the database
        Task SaveChangesAsync();
    }
}