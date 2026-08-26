using Demo.Model.Domain.Validation;
using Demo.Model.Enumerations;
using Demo.Model.Utils;
using Demo.Model.Validation;
using Microsoft.Extensions.Logging;

namespace Demo.Model.Domain;

public class DomainObject
{
    #region Fields

    protected static int UnsavedID = 0;

    #endregion

    #region Properties

    public int Id { get; set; }

    public bool IsDeleted { get; set; }

    #endregion

    #region Constructors

    public DomainObject(int id)
    {
        Id = id;
        IsDeleted = false;
    }

    #endregion

    #region Business Logic

    public virtual void OnCreated()
    {
    }

    public virtual void OnDeleted()
    {
        IsDeleted = true;
    }

    #endregion

    #region Utility Methods

    protected void LogMessageAndThrowEntityNotFoundException(ILogger logger, string message)
    {
        logger.LogDebug(message);
        throw new EntityNotFoundException(message);
    }

    protected void LogMessageAndThrowValidationException<T>(ILogger logger, T errorEnumValue) where T : Enum
    {
        if (logger is not null)
        {
            logger.LogDebug(errorEnumValue.Description());
        }

        throw new ValidationException(errorEnumValue.ToErrorMessage());
    }

    #endregion
}
