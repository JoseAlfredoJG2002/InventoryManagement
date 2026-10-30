using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.Features.Categories.Queries;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Categories
{
    public class GetAllCategoriesHandlerTests
    {
        private readonly Mock<ICategoryQueryRepository> _categoryQueryRepositoryMock;
        private readonly GetAllCategoriesHandler _handler;
        public GetAllCategoriesHandlerTests()
        {
            _categoryQueryRepositoryMock = new Mock<ICategoryQueryRepository>();
            _handler = new GetAllCategoriesHandler(_categoryQueryRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenCategoriesExist_ReturnsCategoryResponses()
        {
            // Arrange
            var firstCategory = new Category("Electronics", "Electronic devices");
            var secondCategory = new Category("Furniture", "Office furniture");
            var categories = new List<Category>
            {
                firstCategory,
                secondCategory
            };
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<Category>)categories);
            // Act
            var result = await _handler.HandleAsync();
            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(firstCategory.Id, result[0].Id);
            Assert.Equal("Electronics", result[0].Name);
            Assert.Equal("Electronic devices", result[0].Description);
            Assert.Equal(secondCategory.Id, result[1].Id);
            Assert.Equal("Furniture", result[1].Name);
            Assert.Equal("Office furniture", result[1].Description);
            _categoryQueryRepositoryMock.Verify(
                repository => repository.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task HandleAsync_WhenNoCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            _categoryQueryRepositoryMock
                .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IReadOnlyList<Category>)new List<Category>());
            // Act
            var result = await _handler.HandleAsync();
            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _categoryQueryRepositoryMock.Verify(
                repository => repository.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
