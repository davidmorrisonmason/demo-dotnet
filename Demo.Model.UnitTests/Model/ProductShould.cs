namespace Demo.Model.UnitTests.Model
{
    public class ProductShould : ModelTest
    {
        [Fact]
        public void StoreCorrectValues_When_UpdateCalled()
        {
            // Arrange
            var original = BuilderFactory.NewProductBuilder()
                .Build();

            var expected = BuilderFactory.NewProductBuilder()
                .BuildFrom(original)
                .With(x => x.Name, "New Name")
                .With(x => x.Price, 99.23m)
                .Build();

            // Act
            original.Update("New Name", 99.23m);

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }
    }
}
