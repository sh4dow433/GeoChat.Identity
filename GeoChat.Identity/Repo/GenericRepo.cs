using GeoChat.Identity.Api.DbAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GeoChat.Identity.Api.Repo;

public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class, new()
{
    protected readonly AppDbContext _context;
    public GenericRepo(AppDbContext context)
    {
        _context = context;  
    }
    public async virtual Task<TEntity> CreateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;  
    }

    public async virtual Task<bool> DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        var rowsDeleted = await _context.SaveChangesAsync();
        if (rowsDeleted > 0)
            return true;
        return false;
    }   

    public async virtual Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async virtual Task<TEntity?> GetAsync(object id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async virtual Task<bool> UpdateAsync(TEntity entity)
    {
        _context.Update(entity);
        var rowsUpdated = await _context.SaveChangesAsync();
        if (rowsUpdated > 0) return true;   
        return false;
    }
}