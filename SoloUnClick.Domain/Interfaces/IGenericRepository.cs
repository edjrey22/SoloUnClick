using System.Linq.Expressions;

namespace SoloUnClick.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    // Métodos de consulta
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(string id); // Para ApplicationUser con string Id
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    // Métodos de escritura
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Métodos auxiliares
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
    // Método para consultas con Include
    Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
}
