using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Categories.Commands;
using Moq;

namespace Inventory.UnitTests.Application.Categories
{
    public class DeleteCategoryHandlerTests
    {
        private readonly Mock<ICategoryCommandRepository> _categoryCommandRepositoryMock;
        private readonly DeleteCategoryHandler _handler;
        public DeleteCategoryHandlerTests()
        {
            _categoryCommandRepositoryMock = new Mock<ICategoryCommandRepository>();
            _handler = new DeleteCategoryHandler(_categoryCommandRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoryExists_DeletesCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _categoryCommandRepositoryMock
                .Setup(repository => repository.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            // Act
            await _handler.HandleAsync(categoryId);
            // Assert
            _categoryCommandRepositoryMock.Verify(
                repository => repository.DeleteAsync(categoryId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoryDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _categoryCommandRepositoryMock
                .Setup(repository => repository.DeleteAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            // Act
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(categoryId));
            // Assert
            Assert.Contains(categoryId.ToString(), exception.Message);
            _categoryCommandRepositoryMock.Verify(
                repository => repository.DeleteAsync(categoryId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
