namespace Demo.DomainServices.Interface.Time;

public interface ITimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
