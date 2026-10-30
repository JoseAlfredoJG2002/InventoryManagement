using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;
using Inventory.Application.Features.Products.Commands;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Products
{
    public class UpdateProductHandlerTests
    {
        [Fact]
        public async Task HandleAsync_ValidRequest_ShouldUpdateProduct()
        {
            // Arrange
            var category = new Category("Electronics");
            var product = new Product(
                "ELEC-007",
                "Old Product Name",
                500m,
                category.Id,
                "Old description");

            product.ApplyInventoryMovement(true, 10);

            var productQueryRepository = new Mock<IProductQueryRepository>();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            productQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            productCommandRepository
                .Setup(repository => repository.UpdateAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateProductHandler(
                productQueryRepository.Object,
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new UpdateProductRequest(
                "Updated Product",
                "Updated description",
                799.99m,
                category.Id);

            // Act
            var result = await handler.HandleAsync(product.Id, request);

            // Assert
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("ELEC-007", result.SKU);
            Assert.Equal("Updated Product", result.Name);
            Assert.Equal("Updated description", result.Description);
            Assert.Equal(799.99m, result.Price);
            Assert.Equal(10, result.StockQuantity);
            Assert.Equal(category.Id, result.CategoryId);
            Assert.True(result.IsActive);

            productCommandRepository.Verify(
                repository => repository.UpdateAsync(
                    product,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task HandleAsync_ProductNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var category = new Category("Electronics");

            var productQueryRepository = new Mock<IProductQueryRepository>();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            productQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    productId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new UpdateProductHandler(
                productQueryRepository.Object,
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new UpdateProductRequest(
                "Updated Product",
                "Updated description",
                799.99m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(productId, request));

            categoryQueryRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            productCommandRepository.Verify(
                repository => repository.UpdateAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_CategoryNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product("ELEC-008", "Monitor", 1500m, Guid.NewGuid());

            var productQueryRepository = new Mock<IProductQueryRepository>();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            productQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    categoryId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new UpdateProductHandler(
                productQueryRepository.Object,
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new UpdateProductRequest(
                "Updated Monitor",
                "Updated description",
                1800m,
                categoryId);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(product.Id, request));

            productCommandRepository.Verify(
                repository => repository.UpdateAsync(
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
            var product = new Product("ELEC-009", "Monitor", 1500m, category.Id);

            var productQueryRepository = new Mock<IProductQueryRepository>();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            productQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = new UpdateProductHandler(
                productQueryRepository.Object,
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new UpdateProductRequest(
                name,
                "Updated description",
                1800m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.HandleAsync(product.Id, request));

            productCommandRepository.Verify(
                repository => repository.UpdateAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_NegativePrice_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var category = new Category("Electronics");
            var product = new Product("ELEC-010", "Monitor", 1500m, category.Id);

            var productQueryRepository = new Mock<IProductQueryRepository>();
            var productCommandRepository = new Mock<IProductCommandRepository>();
            var categoryQueryRepository = new Mock<ICategoryQueryRepository>();

            productQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            categoryQueryRepository
                .Setup(repository => repository.GetByIdAsync(
                    category.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = new UpdateProductHandler(
                productQueryRepository.Object,
                productCommandRepository.Object,
                categoryQueryRepository.Object);

            var request = new UpdateProductRequest(
                "Updated Monitor",
                "Updated description",
                -100m,
                category.Id);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => handler.HandleAsync(product.Id, request));

            productCommandRepository.Verify(
                repository => repository.UpdateAsync(
                    It.IsAny<Product>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

    }
}
