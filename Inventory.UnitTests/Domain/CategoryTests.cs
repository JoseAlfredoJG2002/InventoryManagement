using Inventory.Domain.Entities;

namespace Inventory.UnitTests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void Constructor_WithValidData_CreatesCategory()
        {
            var category = new Category("Electronics", "Electronic devices");

            Assert.NotEqual(Guid.Empty, category.Id);
            Assert.Equal("Electronics", category.Name);
            Assert.Equal("Electronic devices", category.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidName_ThrowsArgumentException(string name)
        {
            Assert.Throws<ArgumentException>(() => new Category(name));
        }

        [Fact]
        public void Constructor_WithNameContainingSpaces_TrimsName()
        {
            var category = new Category("  Electronics  ");

            Assert.Equal("Electronics", category.Name);
        }

        [Fact]
        public void Update_WithValidData_UpdatesCategoryProperties()
        {
            var category = new Category("Electronics", "Old description");

            category.Update("Personal Care", "Personal hygiene products");

            Assert.Equal("Personal Care", category.Name);
            Assert.Equal("Personal hygiene products", category.Description);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            var category = new Category("Electronics");

            Assert.Throws<ArgumentException>(() => category.Update("", "Description"));
        }
    }
}
