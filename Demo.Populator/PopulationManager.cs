using Demo.Infrastructure.Data;
using Demo.Populator.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Populator;

public class PopulationManager : IPopulationManager
{
    private readonly ILogger<PopulationManager> _logger;
    private readonly IClientPopulator _clientPopulator;
    private readonly ICategoryPopulator _categoryPopulator;
    private readonly ApplicationDbContext _dbContext;

    public PopulationManager(
        ILogger<PopulationManager> logger,
        IClientPopulator clientPopulator,
        ICategoryPopulator categoryPopulator,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _clientPopulator = clientPopulator;
        _categoryPopulator = categoryPopulator;
        _dbContext = dbContext;
    }

    public async Task DoPopulation()
    {
        _logger.LogInformation("Truncating database");

        _dbContext.Database.ExecuteSql($"DELETE FROM Categories");
        _dbContext.Database.ExecuteSql($"DELETE FROM Clients");

        _logger.LogInformation("Database truncation complete");

        _logger.LogInformation("Beginning population");

        await InvokePopulator(_clientPopulator, "Clients");

        _categoryPopulator.SetClients(_clientPopulator.ClientsByPlainTextApiKey);

        await InvokePopulator(_categoryPopulator, "Categories");

        _logger.LogInformation("Population complete.");
    }

    private async Task InvokePopulator(IPopulator populator, string type)
    {
        _logger.LogInformation("Populating {Type}", type);

        await populator.Populate();

        _logger.LogInformation("Finished Populating {Type}", type);

    }
}
