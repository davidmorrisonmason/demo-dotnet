using Demo.Model.Domain;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;

namespace Demo.Model.UnitTests.Model
{
    public class CategoryShould : ModelTest
    {
        [Fact]
        public void StoreCorrectValues_When_UpdateCalled()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.Name, "New Name")
                .Build();

            // Act
            original.Update("New Name");

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void StoreCorrectValues_When_UpdateProductCalled_WithExistingProductAndUniqueName()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).With(x => x.Name, "Updated Product").With(x => x.Price, 99.23m).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            // Act
            original.UpdateProduct(original.Products[1].Id, expected.Products[1].Name, expected.Products[1].Price);

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowException_When_UpdateProductCalled_WithUnknownProductId()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            var expectedMessage = "Product with supplied ID '999' does not exist";

            // Act
            var actual = Assert.Throws<EntityNotFoundException>(() => original.UpdateProduct(999, "Updated Product", 99.23m));

            // Assert
            actual.Message.ShouldEqual(expectedMessage);
        }

        [Fact]
        public void ThrowException_When_UpdateProductCalled_WithNameUsedByDifferentProduct()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            var expected = Category.CategoryErrorType.Product_Name_Must_Be_Unique.BuildErrorMessage();

            // Act
            var actual = Assert.Throws<ValidationException>(() => original.UpdateProduct(original.Products[0].Id, original.Products[1].Name, 99.23m));

            // Assert
            actual.ErrorMessages.ShouldBeEquivalentTo([expected]);
        }

        [Fact]
        public void AllowExistingName_When_UpdateProductCalled_ForSameProduct()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).With(x => x.Price, 99.23m).Build(),
                    BuilderFactory.NewProductBuilder(3, 3).Build()
                ])
                .Build();

            // Act
            original.UpdateProduct(original.Products[1].Id, original.Products[1].Name, expected.Products[1].Price);

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowException_When_AddProductCalled_WithNonUniqueProductName()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).Build(),
                    BuilderFactory.NewProductBuilder(3).Build()
                ])
                .Build();

            var expected = Category.CategoryErrorType.Product_Name_Must_Be_Unique.BuildErrorMessage();

            // Act
            var actual = Assert.Throws<ValidationException>(() => original.AddProduct(original.Products[1].Name, 23.2m));

            // Assert
            actual.ErrorMessages.ShouldBeEquivalentTo([expected]);
        }

        [Fact]
        public void AddNewProduct_When_AddProductCalled_WithUniqueProductName()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).Build(),
                    BuilderFactory.NewProductBuilder(3).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).Build(),
                    BuilderFactory.NewProductBuilder(3).Build(),
                    BuilderFactory.NewProductBuilder(4).Build()
                ])
                .Build();

            // Act
            original.AddProduct(expected.Products[3].Name, expected.Products[3].Price);

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void AddNewSubCategory_When_AddSubCategoryCalled_WithUniqueName()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1).Build(),
                    BuilderFactory.NewCategoryBuilder(2).Build(),
                    BuilderFactory.NewCategoryBuilder(3).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder().BuildFrom(original)
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder().BuildFrom(original.SubCategories[0]).Build(),
                    BuilderFactory.NewCategoryBuilder().BuildFrom(original.SubCategories[1]).Build(),
                    BuilderFactory.NewCategoryBuilder().BuildFrom(original.SubCategories[2]).Build(),
                    BuilderFactory.NewCategoryBuilder(4)
                        .With(x => x.Id, 0)
                        .With(x => x.ParentCategoryId, original.Id)
                        .With(x => x.ClientId, original.ClientId)
                        .With(x => x.Name, "Category 4")
                        .Build()
                ])
                .Build();

            // Act
            var actual = original.AddSubCategory("Category 4");

            // Assert
            actual.ShouldBeEquivalentTo(expected.SubCategories[3]);
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowException_When_AddSubCategoryCalled_WithNonUniqueName()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1, 1).Build(),
                    BuilderFactory.NewCategoryBuilder(2, 2).Build(),
                    BuilderFactory.NewCategoryBuilder(3, 3).Build()
                ])
                .Build();

            var expected = Category.CategoryErrorType.SubCategory_Name_Must_Be_Unique.BuildErrorMessage();

            // Act
            var actual = Assert.Throws<ValidationException>(() => original.AddSubCategory(original.SubCategories[1].Name));

            // Assert
            actual.ErrorMessages.ShouldBeEquivalentTo([expected]);
        }

        [Fact]
        public void SoftDeleteSubCategoryAndItsProducts_When_RemoveSubCategoryCalled_WithExistingSubCategoryId()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1, 1)
                        .With(x => x.Products,
                        [
                            BuilderFactory.NewProductBuilder(1, 1).Build(),
                            BuilderFactory.NewProductBuilder(2, 2).Build()
                        ])
                        .Build(),
                    BuilderFactory.NewCategoryBuilder(2, 2).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder()
                        .BuildFrom(original.SubCategories[0])
                        .With(x => x.IsDeleted, true)
                        .With(x => x.Products,
                        [
                            BuilderFactory.NewProductBuilder()
                                .BuildFrom(original.SubCategories[0].Products[0])
                                .With(x => x.IsDeleted, true)
                                .Build(),
                            BuilderFactory.NewProductBuilder()
                                .BuildFrom(original.SubCategories[0].Products[1])
                                .With(x => x.IsDeleted, true)
                                .Build()
                        ])
                        .Build(),
                    BuilderFactory.NewCategoryBuilder()
                        .BuildFrom(original.SubCategories[1])
                        .Build()
                ])
                .Build();

            // Act
            original.RemoveSubCategory(original.SubCategories[0].Id);

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowException_When_RemoveSubCategoryCalled_WithUnknownSubCategoryId()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1, 1).Build(),
                    BuilderFactory.NewCategoryBuilder(2, 2).Build()
                ])
                .Build();

            var expectedMessage = "Subcategory with supplied ID '999' does not exist";

            // Act
            var actual = Assert.Throws<EntityNotFoundException>(() => original.RemoveSubCategory(999));

            // Assert
            actual.Message.ShouldEqual(expectedMessage);
        }

        [Fact]
        public void SoftDeleteCategoryAndItsProducts_When_OnDeletedCalled()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder(1, 1).Build(),
                    BuilderFactory.NewProductBuilder(2, 2).Build()
                ])
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.IsDeleted, true)
                .With(x => x.Products,
                [
                    BuilderFactory.NewProductBuilder()
                        .BuildFrom(original.Products[0])
                        .With(x => x.IsDeleted, true)
                        .Build(),
                    BuilderFactory.NewProductBuilder()
                        .BuildFrom(original.Products[1])
                        .With(x => x.IsDeleted, true)
                        .Build()
                ])
                .Build();

            // Act
            original.OnDeleted();

            // Assert
            original.ShouldBeEquivalentTo(expected);
        }
    }
}
