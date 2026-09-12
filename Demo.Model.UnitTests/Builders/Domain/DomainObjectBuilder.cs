using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Demo.Model.UnitTests.Builders.Domain;

public class DomainObjectBuilder<T> : Builder<T> where T : DomainObject
{
    protected BuilderFactory BuilderFactory { get; set; }

    public DomainObjectBuilder(BuilderFactory builderFactory, T target) : base(target)
    {
        BuilderFactory = builderFactory;
    }

    protected DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }

    public Builder<T> WithDbContextOptions(DbContextOptions<ApplicationDbContext> dbContextOptions)
    {
        DbContextOptions = dbContextOptions;
        return this;
    }

    public new DomainObjectBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty newValue)
    {
        // this method is implemented in order to return DomainObjectBuilder as the type for fluent property setting
        base.With(expression, newValue);
        return this;
    }

    public new DomainObjectBuilder<T> BuildFrom(T source)
    {
        // this method is implemented in order to return DomainObjectBuilder as the type for fluent property setting
        base.BuildFrom(source);

        return this;
    }

    public DomainObjectBuilder<T> WithDeletedStatus()
    {
        With(x => x.IsDeleted, true);
        return this;
    }

    public virtual DomainObjectBuilder<T> WithNextId(int additionalToAdd = 1)
    {
        using var dbContext = new ApplicationDbContext(DbContextOptions);
        var set = dbContext.Set<T>();
        var maxId = set.Any() ? set.Max(x => x.Id) : 0;
        With(x => x.Id, maxId + additionalToAdd);
        return this;
    }

    public virtual DomainObjectBuilder<T> WithMaxId()
    {
        using var dbContext = new ApplicationDbContext(DbContextOptions);
        var set = dbContext.Set<T>();
        var maxId = set.Any() ? set.Max(x => x.Id) : 0;
        With(x => x.Id, maxId);
        return this;
    }

    public virtual T BuildAndPersist()
    {
        using var dbContext = new ApplicationDbContext(DbContextOptions);
        Persist(dbContext);
        dbContext.SaveChanges();

        return Build();
    }

    protected virtual void Persist(ApplicationDbContext applicationDbContext)
    {
        throw new Exception($"Persist method not defined for type {typeof(T).Name}");
    }

}
