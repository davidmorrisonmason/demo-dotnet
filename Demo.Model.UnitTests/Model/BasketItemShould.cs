using Demo.Model.Domain.Checkout;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;

namespace Demo.Model.UnitTests.Model;

public class BasketItemShould : ModelTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ThrowValidationException_When_OnCreatedCalled_WithNonPositiveQuantity(int quantity)
    {
        // Arrange
        var basketItem = BuilderFactory.NewBasketItemBuilder()
            .With(x => x.Quantity, quantity)
            .Build();

        // Act
        var actual = Assert.Throws<ValidationException>(() => basketItem.OnCreated());

        // Assert
        actual.ErrorMessages.ShouldBeEquivalentTo(
            BasketItem.BasketItemErrorType.Quantity_Must_Be_Greater_Than_Zero.BuildErrorMessages());
    }

    [Fact]
    public void NotThrowValidationException_When_OnCreatedCalled_WithPositiveQuantity()
    {
        // Arrange
        var basketItem = BuilderFactory.NewBasketItemBuilder()
            .With(x => x.Quantity, 1)
            .Build();

        // Act
        basketItem.OnCreated();

        // Assert - nothing to do if no exception was thrown
    }

    [Fact]
    public void ThrowValidationException_When_AddQuantityCalled_WithQuantityThatResultsInNoQuantity()
    {
        // Arrange
        var basketItem = BuilderFactory.NewBasketItemBuilder()
            .With(x => x.Quantity, 0)
            .Build();

        // Act
        var actual = Assert.Throws<ValidationException>(() => basketItem.AddQuantity(0));

        // Assert
        actual.ErrorMessages.ShouldBeEquivalentTo(
            BasketItem.BasketItemErrorType.Quantity_Must_Be_Greater_Than_Zero.BuildErrorMessages());
    }

}
