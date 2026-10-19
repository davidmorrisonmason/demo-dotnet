using Demo.DomainServices.Encryption;
using Demo.Model.Domain.Exceptions;
using Demo.Model.UnitTests.Validation;

namespace Demo.Model.UnitTests.DomainServices;

public class EncryptionServiceShould
{
    private readonly EncryptionService service = new();

    [Fact]
    public void ProduceCorrectFingerprint_WhenFingerprintCalled_WithKnownSha256Fingerprint()
    {
        // Arrange
        var expected = "BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD";

        // Act
        var actual = service.Fingerprint("abc");

        // Assert
        actual.ShouldEqual(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowException_WhenFingerprintCalled_WithBlankPlainText(string? plainText)
    {
        var exception = Assert.Throws<SecurityException>(() => service.Fingerprint(plainText!));
        exception.ErrorMessages.ShouldBeEquivalentTo(EncryptionErrorType.Plain_Text_Required.BuildErrorMessages());
    }

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
