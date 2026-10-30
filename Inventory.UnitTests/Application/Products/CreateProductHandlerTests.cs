using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;
using Inventory.Application.Features.Products.Commands;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Products
{
    public class CreateProductHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ValidRequest_ShouldCreateProduct()
        {
            // Arrange
            var category = new Category("Electronics");
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            productCommandRepository
                .Setup(repository => repository.AddAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new CreateProductHandler(
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new CreateProductRequest(
                "ELEC-003",
                "Wireless Keyboard",
                "Mechanical keyboard",
                899.99m,
                category.Id);

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("ELEC-003", result.SKU);
            Assert.Equal("Wireless Keyboard", result.Name);
            Assert.Equal("Mechanical keyboard", result.Description);
            Assert.Equal(899.99m, result.Price);
            Assert.Equal(0, result.StockQuantity);
            Assert.Equal(category.Id, result.CategoryId);
            Assert.True(result.IsActive);

            categoryQueryRepository.Verify(
                repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            productCommandRepository.Verify(
                repository => repository.AddAsync(
                    It.Is<Product>(product =>
                        product.SKU == "ELEC-003" &&
                        product.Name == "Wireless Keyboard" &&
                        product.Price == 899.99m &&
                        product.CategoryId == category.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_CategoryNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    categoryId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new CreateProductHandler(
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new CreateProductRequest(
                "ELEC-004",
                "Monitor",
                "Computer monitor",
                2500m,
                categoryId);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(request));

            productCommandRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_InvalidSKU_ShouldThrowArgumentException(string sku)
        {
            // Arrange
            var category = new Category("Electronics");
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = new CreateProductHandler(
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new CreateProductRequest(
                sku,
                "Wireless Keyboard",
                "Mechanical keyboard",
                899.99m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(request));

            productCommandRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_InvalidName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var category = new Category("Electronics");
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = new CreateProductHandler(
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new CreateProductRequest(
                "ELEC-005",
                name,
                "Mechanical keyboard",
                899.99m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(request));

            productCommandRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_NegativePrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var category = new Category("Electronics");
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = new CreateProductHandler(
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new CreateProductRequest(
                "ELEC-006",
                "Wireless Keyboard",
                "Mechanical keyboard",
                -100m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => handler.HandleAsync(request));

            productCommandRepository.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
