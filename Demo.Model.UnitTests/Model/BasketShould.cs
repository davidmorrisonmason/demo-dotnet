using Demo.Model.Domain.Checkout;

namespace Demo.Model.UnitTests.Model
{
    public class BasketShould : ModelTest
    {
        [Fact]
        public void StoreCorrectStatus_When_CompleteCalled()
        {
            // Arrange
            var original = BuilderFactory.NewBasketBuilder()
                .Build();

            var expected = BuilderFactory.NewBasketBuilder()
                .BuildFrom(original)
                .With(x => x.Status, BasketStatus.Complete)
                .Build();

            // Act
            original.Complete();

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }
    }
}
