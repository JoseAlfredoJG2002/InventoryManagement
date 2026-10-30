using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;
using Inventory.Application.Features.Categories.Commands;
using Inventory.Domain.Entities;
using Moq;

namespace Inventory.UnitTests.Application.Categories
{
    public class CreateCategoryHandlerTests
    {
        private readonly Mock<ICategoryCommandRepository> _categoryCommandRepositoryMock;
        private readonly CreateCategoryHandler _handler;
        public CreateCategoryHandlerTests()
        {
            _categoryCommandRepositoryMock = new Mock<ICategoryCommandRepository>();
            _handler = new CreateCategoryHandler(_categoryCommandRepositoryMock.Object);
        }
        [Fact]
        public async Task HandleAsync_WhenRequestIsValid_CreatesCategory()
        {
            // Arrange
            var request = new CreateCategoryRequest("Electronics", "Electronic devices");
            _categoryCommandRepositoryMock
                .Setup(repository => repository.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await _handler.HandleAsync(request);
            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Electronics", result.Name);
            Assert.Equal("Electronic devices", result.Description);
            _categoryCommandRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.Is<Category>(category =>
                        category.Name == "Electronics" &&
                        category.Description == "Electronic devices"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task HandleAsync_WhenNameIsEmpty_ThrowsArgumentException(string name)
        {
            // Arrange
            var request = new CreateCategoryRequest(name, "Category description");
            // Act
            await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.HandleAsync(request));
            // Assert
            _categoryCommandRepositoryMock.Verify(
                repository => repository.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
