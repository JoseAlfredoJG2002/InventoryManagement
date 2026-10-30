using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;
using Inventory.Application.Features.Categories.Commands;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Categories
{
    public class UpdateCategoryHandlerTests
    {
        private readonly Mock<ICategoryQueryRepository> _categoryQueryRepositoryMock;
        private readonly Mock<ICategoryCommandRepository> _categoryCommandRepositoryMock;
        private readonly UpdateCategoryHandler _handler;
        public UpdateCategoryHandlerTests()
        {
            _categoryQueryRepositoryMock = new Mock<ICategoryQueryRepository>();
            _categoryCommandRepositoryMock = new Mock<ICategoryCommandRepository>();
            _handler = new UpdateCategoryHandler(
                _categoryQueryRepositoryMock.Object,
                _categoryCommandRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoryExists_UpdatesCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Old name", "Old description");
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var request = new UpdateCategoryRequest("Electronics", "Electronic devices");
            _categoryCommandRepositoryMock
                .Setup(repository => repository.UpdateAsync(category, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _handler.HandleAsync(categoryId, request);
            // Assert
            Assert.Equal(category.Id, result.Id);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic devices", result.Description);
            _categoryCommandRepositoryMock.Verify(
                repository => repository.UpdateAsync(category, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoryDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);
            var request = new UpdateCategoryRequest("Electronics", "Electronic devices");
            // Act
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(categoryId, request));
            // Assert
            _categoryCommandRepositoryMock.Verify(
                repository => repository.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_WhenNameIsInvalid_ThrowsArgumentException(string name)
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Electronics", "Electronic devices");
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var request = new UpdateCategoryRequest(name, "Updated description");
            // Act
            await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.HandleAsync(categoryId, request));
            // Assert
            _categoryCommandRepositoryMock.Verify(
                repository => repository.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
