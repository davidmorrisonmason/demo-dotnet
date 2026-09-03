using Demo.DomainServices.Encryption;
using Demo.Model.Domain.Exceptions;
using Demo.Model.UnitTests.Validation;

namespace Demo.Model.UnitTests.DomainServices;

public class EncryptionServiceShould
{
    private readonly EncryptionService service = new();

    [Fact]
    public void EncryptCorrectlyWithDifferentSalts_OnSuccessiveCallsWithSamePlainText()
    {
        // Act
        string result = service.OneWayHash("plain text");
        string result2 = service.OneWayHash("plain text");

        // Assert
        result.ShouldNotEqual(result2);
        service.Verify("plain text", result).ShouldBeTrue();

        service.Verify("plain text", result2).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowSecurityExceptionForNullOrEmptyPlainText(string? plainText)
    {
        // Act
        var exception = Assert.Throws<SecurityException>(() => service.OneWayHash(plainText!));

        // Aseert
        exception.ErrorMessages.ShouldBeEquivalentTo(EncryptionErrorType.Plain_Text_Required.BuildErrorMessages());
    }
}
