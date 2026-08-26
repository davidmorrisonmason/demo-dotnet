using Demo.DomainServices.Interface.Transaction;

namespace Demo.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(
        ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Execute an asynchronous unit of work within a transaction
    /// </summary>
    /// <param name="action">The async action to invoke</param>
    public async Task Execute(Func<Task> action, bool transactional = true)
    {
        if (transactional)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            await action.Invoke();
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        else
        {
            await action.Invoke();
        }
    }

    /// <summary>
    /// Execute a synchronous unit of work within a transaction
    /// </summary>
    /// <param name="action">The synchornous action to invoke</param>
    public async Task Execute(Action action, bool transactional = true)
    {
        if (transactional)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            action.Invoke();
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        else
        {
            action.Invoke();
        }
    }

    public async Task Commit()
    {
        await _dbContext.SaveChangesAsync();
    }
}

