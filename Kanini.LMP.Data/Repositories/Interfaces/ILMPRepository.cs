using System.Linq.Expressions;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface ILMPRepository<T, TId> where T : class
    {
        // Create
        Task<T> AddAsync(T entity);

        // Read with predicates
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T?> GetByIdAsync(TId id);
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

        // Update with predicate
        Task<T> UpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null);

        // Delete with predicate
        Task DeleteAsync(TId id);
        Task DeleteAsync(Expression<Func<T, bool>> predicate);

        // Utility
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
