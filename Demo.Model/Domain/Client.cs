using Newtonsoft.Json;

namespace Demo.Model.Domain;

public class Client : DomainObject, IAggregateRoot
{
    #region Properties

    public string Name { get; set; }
    public string ApiKey { get; set; }

    #endregion

    #region Constructors

    public Client() : this(string.Empty, string.Empty)
    {

    }

    public Client(string name, string apiKey) : this(UnsavedID, name, apiKey)
    {
    }

    [JsonConstructor]
    public Client(int id, string name, string apiKey) : base(id)
    {
        Name = name;
        ApiKey = apiKey;
    }

    #endregion

}
