using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Categories.Queries;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Categories
{
    public class GetCategoryByIdHandlerTests
    {
        private readonly Mock<ICategoryQueryRepository> _categoryQueryRepositoryMock;
        private readonly GetCategoryByIdHandler _handler;
        public GetCategoryByIdHandlerTests()
        {
            _categoryQueryRepositoryMock = new Mock<ICategoryQueryRepository>();
            _handler = new GetCategoryByIdHandler(_categoryQueryRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoryExists_ReturnsCategoryResponse()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices");
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            // Act
            var result = await _handler.HandleAsync(category.Id);
            // Assert
            Assert.Equal(category.Id, result.Id);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic devices", result.Description);
            _categoryQueryRepositoryMock.Verify(
                repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()),
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
            // Act
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.HandleAsync(categoryId));
            // Assert
            Assert.Contains(categoryId.ToString(), exception.Message);
            _categoryQueryRepositoryMock.Verify(
                repository => repository.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
