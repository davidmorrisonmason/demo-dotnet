using Demo.Model.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Model.Domain;

public class Category : DomainObject, IAggregateRoot
{
    #region Fields

    private static readonly ILogger<Category> _logger = DomainContext.Instance.CreateLogger<Category>();

    #endregion

    #region Constructors

    public Category(
        string name) : this(UnsavedID, name)
    {
    }

    [JsonConstructor]
    public Category(
        int id,
        string name) : base(id)
    {
        Name = name;
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    #endregion

    #region Business Logic 
    public void Update(string name)
    {
        Name = name;
    }

    #endregion

    #region Error Types

    public enum CategoryErrorType
    {
    }

    #endregion
}
