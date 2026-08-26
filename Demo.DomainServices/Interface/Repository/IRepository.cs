using Demo.Model.Domain;

namespace Demo.DomainServices.Interface.Repository;

public interface IRepository<T> where T : DomainObject, IAggregateRoot
{
    Task<IEnumerable<T>> GetAll();
    Task<IEnumerable<T>> GetAllExcluding(int idToExclude);
    Task<T?> Get(int id);
    Task Add(T entity);
    Task Remove(T entity);
}
