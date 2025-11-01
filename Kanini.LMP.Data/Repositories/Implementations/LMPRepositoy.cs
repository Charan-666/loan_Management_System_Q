using Microsoft.EntityFrameworkCore;
using Kanini.LMP.Data.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Reflection;
using Kanini.LMP.Data.Data;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class LMPRepositoy<T, TId> : ILMPRepository<T, TId> where T : class
    {
        protected readonly LmpDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public LMPRepositoy(LmpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var result = await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                var query = _dbSet.AsNoTracking();

                if (predicate != null)
                    query = query.Where(predicate);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<T?> GetByIdAsync(TId id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving {typeof(T).Name} with ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                T? existingEntity;

                if (predicate != null)
                {
                    // Update by predicate
                    existingEntity = await _dbSet.FirstOrDefaultAsync(predicate);
                    if (existingEntity == null)
                        throw new ArgumentException($"{typeof(T).Name} not found with the given predicate");
                }
                else
                {
                    // Update by ID (default behavior)
                    var keyProperty = GetKeyProperty();
                    var keyValue = keyProperty.GetValue(entity);
                    existingEntity = await _dbSet.FindAsync(keyValue);
                    if (existingEntity == null)
                        throw new ArgumentException($"{typeof(T).Name} with ID {keyValue} not found");
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingEntity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task DeleteAsync(TId id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting {typeof(T).Name} with ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var entities = await _dbSet.Where(predicate).ToListAsync();
                if (entities.Any())
                {
                    _dbSet.RemoveRange(entities);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.AnyAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking existence: {ex.Message}", ex);
            }
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            try
            {
                return predicate == null
                    ? await _dbSet.CountAsync()
                    : await _dbSet.CountAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting records: {ex.Message}", ex);
            }
        }

        private PropertyInfo GetKeyProperty()
        {
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true).Length > 0);

            return keyProperty ?? throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a Key attribute");
        }
    }
}
