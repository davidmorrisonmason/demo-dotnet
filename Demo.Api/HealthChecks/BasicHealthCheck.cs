using Demo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Demo.Api.HealthChecks
{
    public class BasicHealthCheck : IHealthCheck
    {
        public ApplicationDbContext _dbContext;

        public BasicHealthCheck(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // touch the DB to ensure connection is OK
                var category = await _dbContext.Categories.FirstOrDefaultAsync();
                return HealthCheckResult.Healthy("Healthy");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
