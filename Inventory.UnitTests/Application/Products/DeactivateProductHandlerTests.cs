using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Products.Commands;
using Moq;

namespace Inventory.UnitTests.Application.Products
{
    public class DeactivateProductHandlerTests
    {
        private readonly Mock<IProductCommandRepository> _productCommandRepositoryMock;
        private readonly DeactivateProductHandler _handler;
        public DeactivateProductHandlerTests()
        {
            _productCommandRepositoryMock = new Mock<IProductCommandRepository>();
            _handler = new DeactivateProductHandler(_productCommandRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenProductExists_DeactivatesProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productCommandRepositoryMock
                .Setup(repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            // Act
            await _handler.HandleAsync(productId);
            // Assert
            _productCommandRepositoryMock.Verify(
                repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productCommandRepositoryMock
                .Setup(repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // Act
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(productId));
            // Assert
            Assert.Contains(productId.ToString(), exception.Message);
            _productCommandRepositoryMock.Verify(
                repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenRepositoryReturnsFalse_DoesNotRetryDeactivation()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productCommandRepositoryMock
                .Setup(repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // Act
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(productId));
            // Assert
            _productCommandRepositoryMock.Verify(
                repository => repository.DeactivateAsync(productId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
