using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Features.Inventory.Commands;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Inventory
{
    public class RegisterInventoryMovementHandlerTests
    {
        [Fact]
        public async Task HandleAsync_EntryMovement_ShouldIncreaseProductStock()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product("TEST-001", "Test Product", 100m, categoryId);
            product.ApplyInventoryMovement(true, 10);

            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            movementRepository
                .Setup(repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                product.Id,
                InventoryMovementType.Entry,
                5,
                "Purchase order");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(15, product.StockQuantity);
            Assert.Equal(15, result.CurrentStock);
            Assert.Equal(InventoryMovementType.Entry, result.Type);
            Assert.Equal(5, result.Quantity);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    product,
                    It.Is<InventoryMovement>(movement =>
                        movement.Type == InventoryMovementType.Entry &&
                        movement.Quantity == 5),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ExitMovement_ShouldDecreaseProductStock()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product("TEST-002", "Test Product", 100m, categoryId);
            product.ApplyInventoryMovement(true, 10);

            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            movementRepository
                .Setup(repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                product.Id,
                InventoryMovementType.Exit,
                4,
                "Sales order");

            // Act
            var result = await handler.HandleAsync(request);

            // Assert
            Assert.Equal(6, product.StockQuantity);
            Assert.Equal(6, result.CurrentStock);
            Assert.Equal(InventoryMovementType.Exit, result.Type);
            Assert.Equal(4, result.Quantity);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    product,
                    It.Is<InventoryMovement>(movement =>
                        movement.Type == InventoryMovementType.Exit &&
                        movement.Quantity == 4),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_InvalidMovementType_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                Guid.NewGuid(),
                (InventoryMovementType)999,
                5,
                "Invalid movement");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => handler.HandleAsync(request));

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ZeroQuantity_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                Guid.NewGuid(),
                InventoryMovementType.Entry,
                0,
                "Invalid quantity");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => handler.HandleAsync(request));

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_NegativeQuantity_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                Guid.NewGuid(),
                InventoryMovementType.Entry,
                -5,
                "Invalid quantity");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => handler.HandleAsync(request));

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ProductNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                Guid.NewGuid(),
                InventoryMovementType.Entry,
                5,
                "Purchase order");

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.HandleAsync(request));

            productRepository.Verify(
                repository => repository.GetByIdAsync(
                    request.ProductId,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_InactiveProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product("TEST-003", "Inactive Product", 100m, categoryId);
            product.Deactivate();

            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                product.Id,
                InventoryMovementType.Entry,
                5,
                "Purchase order");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.HandleAsync(request));

            Assert.Equal(0, product.StockQuantity);
            Assert.False(product.IsActive);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_InsufficientStock_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var product = new Product("TEST-004", "Test Product", 100m, categoryId);
            product.ApplyInventoryMovement(true, 10);

            var productRepository = new Mock<IProductQueryRepository>();
            var movementRepository = new Mock<IInventoryMovementCommandRepository>();

            productRepository
                .Setup(repository => repository.GetByIdAsync(
                    product.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new RegisterInventoryMovementHandler(
                productRepository.Object,
                movementRepository.Object);

            var request = new CreateInventoryMovementRequest(
                product.Id,
                InventoryMovementType.Exit,
                11,
                "Sales order");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.HandleAsync(request));

            Assert.Equal(10, product.StockQuantity);

            movementRepository.Verify(
                repository => repository.RegisterAsync(
                    It.IsAny<Product>(),
                    It.IsAny<InventoryMovement>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}