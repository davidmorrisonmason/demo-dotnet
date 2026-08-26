using Demo.Model.Logging;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Model;

public class ModelTest : Test
{
    public ModelTest()
    {
        var mockDomainContext = Substitute.For<IDomainContext>();
        mockDomainContext.CreateLogger<object>().Returns(Substitute.For<ILogger<object>>());
        DomainContext.Setup(mockDomainContext);
    }
}
