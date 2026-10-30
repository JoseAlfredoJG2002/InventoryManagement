using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Products.Queries;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Products
{
    public class GetProductByIdHandlerTests
    {
        private readonly Mock<IProductQueryRepository> _productQueryRepositoryMock;
        private readonly GetProductByIdHandler _handler;
        public GetProductByIdHandlerTests()
        {
            _productQueryRepositoryMock = new Mock<IProductQueryRepository>();
            _handler = new GetProductByIdHandler(_productQueryRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenProductExists_ReturnsProductResponse()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product(
                "SKU-001",
                "Laptop",
                15000m,
                categoryId,
                "Business laptop");
            product.ApplyInventoryMovement(true, 12);
            _productQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            // Act
            var result = await _handler.HandleAsync(product.Id);
            // Assert
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("SKU-001", result.SKU);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal("Business laptop", result.Description);
            Assert.Equal(15000m, result.Price);
            Assert.Equal(12, result.StockQuantity);
            Assert.Equal(categoryId, result.CategoryId);
            Assert.True(result.IsActive);
            _productQueryRepositoryMock.Verify(
                repository => repository.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);
            // Act
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(productId));
            // Assert
            Assert.Contains(productId.ToString(), exception.Message);
            _productQueryRepositoryMock.Verify(
                repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
