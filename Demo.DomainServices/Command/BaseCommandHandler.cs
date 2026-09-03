using Demo.DomainServices.Interface.Transaction;
using Microsoft.Extensions.Logging;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.DomainServices.Command;

public abstract class BaseCommandHandler
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    protected ILogger Logger => _logger;
    protected IUnitOfWork UnitOfWork => _unitOfWork;
    protected IRequestContext RequestContext { get; }

    /// <summary>
    /// Default command behaviour is transactional. Can be overridden if necessary
    /// </summary>
    protected virtual bool Transactional => true;

    public BaseCommandHandler(
        ILogger logger,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext)
    {
        _logger = logger; 
        _unitOfWork = unitOfWork;
        RequestContext = requestContext;
    }
}
