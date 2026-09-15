using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Microsoft.EntityFrameworkCore;

using Demo.DomainServices.Interface.Context;

namespace Demo.Infrastructure.Repository;

public class Repository<T> : IRepository<T> where T : DomainObject, IAggregateRoot
{
    protected readonly ApplicationDbContext _db;
    protected IRequestContext RequestContext { get; }
    internal DbSet<T> dbSet;

    protected IQueryable<T> NonDeletedEntities => dbSet.AsQueryable()
        .Where(x => !x.IsDeleted);

    public Repository(ApplicationDbContext db, IRequestContext requestContext)
    {
        _db = db;
        RequestContext = requestContext;
        this.dbSet = _db.Set<T>();
    }

    public virtual Task Add(T entity)
    {
        dbSet.Add(entity);

        return Task.CompletedTask;

    }

    public virtual async Task<T?> Get(int id)
    {
        IQueryable<T> query = NonDeletedEntities;
        query = query.Where(x => x.Id == id);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        IQueryable<T> query = NonDeletedEntities;

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllExcluding(int idToExclude)
    {
        IQueryable<T> query = NonDeletedEntities;
        query = query.Where(x => x.Id != idToExclude);

        return await query.ToListAsync();
    }

    public Task Remove(T entity)
    {
        dbSet.Remove(entity);

        return Task.CompletedTask;
    }
}
