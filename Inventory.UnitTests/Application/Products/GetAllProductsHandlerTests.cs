using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Products.Queries;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Products
{
    public class GetAllProductsHandlerTests
    {
        private readonly Mock<IProductQueryRepository> _productQueryRepositoryMock;
        private readonly GetAllProductsHandler _handler;
        public GetAllProductsHandlerTests()
        {
            _productQueryRepositoryMock = new Mock<IProductQueryRepository>();
            _handler = new GetAllProductsHandler(_productQueryRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenProductsExist_ReturnsProductResponses()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var firstProduct = new Product(
                "SKU-001",
                "Laptop",
                15000m,
                categoryId,
                "Business laptop");
            firstProduct.ApplyInventoryMovement(true, 10);
            var secondProduct = new Product(
                "SKU-002",
                "Keyboard",
                800m,
                categoryId,
                "Mechanical keyboard");
            secondProduct.Deactivate();
            var products = new List<Product> { firstProduct, secondProduct };
            _productQueryRepositoryMock
                .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<Product>)products);
            // Act
            var result = await _handler.HandleAsync();
            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(firstProduct.Id, result[0].Id);
            Assert.Equal("SKU-001", result[0].SKU);
            Assert.Equal("Laptop", result[0].Name);
            Assert.Equal("Business laptop", result[0].Description);
            Assert.Equal(15000m, result[0].Price);
            Assert.Equal(10, result[0].StockQuantity);
            Assert.Equal(categoryId, result[0].CategoryId);
            Assert.True(result[0].IsActive);
            Assert.Equal(secondProduct.Id, result[1].Id);
            Assert.Equal("SKU-002", result[1].SKU);
            Assert.Equal("Keyboard", result[1].Name);
            Assert.False(result[1].IsActive);
            _productQueryRepositoryMock.Verify(
                repository => repository.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenNoProductsExist_ReturnsEmptyList()
        {
            // Arrange
            _productQueryRepositoryMock
                .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<Product>)new List<Product>());
            // Act
            var result = await _handler.HandleAsync();
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _productQueryRepositoryMock.Verify(
                repository => repository.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
