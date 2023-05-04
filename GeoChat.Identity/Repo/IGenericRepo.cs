using System.Linq.Expressions;

namespace GeoChat.Identity.Api.Repo;

public interface IGenericRepo<TEntity> where TEntity : class, new()
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TEntity entity);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetAsync(object id);
}
