using Demo.Model.Domain;

namespace Demo.DomainServices.Interface.Repository;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllByName(string name);
    Task<IEnumerable<Category>> GetAllByNameExcludingId(string name, int idToExclude);
}
