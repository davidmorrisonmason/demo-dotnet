using Demo.DomainServices.Interface.Transaction;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command;

public abstract class BaseCommandHandler
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    protected ILogger Logger => _logger;
    protected IUnitOfWork UnitOfWork => _unitOfWork;    

    /// <summary>
    /// Default command behaviour is transactional. Can be overridden if necessary
    /// </summary>
    protected virtual bool Transactional => true;

    public BaseCommandHandler(
        ILogger logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger; 
        _unitOfWork = unitOfWork;
    }
}
