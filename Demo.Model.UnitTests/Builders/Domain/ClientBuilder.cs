using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain;

namespace Demo.Model.UnitTests.Builders.Domain;

public class ClientBuilder : DomainObjectBuilder<Client>
{
    public ClientBuilder(BuilderFactory builderFactory, int databaseSeed, int propertySeed)
        : base(builderFactory, new Client(databaseSeed, $"Client {propertySeed}", $"api-key-{propertySeed}"))
    {
    }

    protected override void Persist(ApplicationDbContext applicationDbContext)
    {
        applicationDbContext.Clients.Add(Target);
    }
}
