using Demo.DomainServices.Interface.Time;

namespace Demo.DomainServices.Time;

public class TimeService : ITimeService
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;
}
