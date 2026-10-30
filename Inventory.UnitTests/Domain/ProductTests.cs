using Inventory.Domain.Entities;

namespace Inventory.UnitTests.Domain
{
    public class ProductTests
    {
        private static Product CreateProduct()
        {
            return new Product("ELEC-001", "Wireless Mouse", 299.99m, Guid.NewGuid(), "Wireless mouse");
        }

        [Fact]
        public void Constructor_WithValidData_CreatesProduct()
        {
            var categoryId = Guid.NewGuid();
            var product = new Product(" ELEC-001 ", " Wireless Mouse ", 299.99m, categoryId, "Wireless mouse");
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal("ELEC-001", product.SKU);
            Assert.Equal("Wireless Mouse", product.Name);
            Assert.Equal(299.99m, product.Price);
            Assert.Equal(categoryId, product.CategoryId);
            Assert.Equal("Wireless mouse", product.Description);
            Assert.Equal(0, product.StockQuantity);
            Assert.True(product.IsActive);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidSku_ThrowsArgumentException(string sku)
        {
            var exception = Assert.Throws<ArgumentException>(() => new Product(sku, "Wireless Mouse", 299.99m, Guid.NewGuid()));
            Assert.Equal("sku", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidName_ThrowsArgumentException(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() => new Product("ELEC-001", name, 299.99m, Guid.NewGuid()));
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentOutOfRangeException()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Product("ELEC-001", "Wireless Mouse", -1m, Guid.NewGuid()));
            Assert.Equal("price", exception.ParamName);
        }

        [Fact]
        public void Constructor_WithEmptyCategoryId_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => new Product("ELEC-001", "Wireless Mouse", 299.99m, Guid.Empty));
            Assert.Equal("categoryId", exception.ParamName);
        }

        [Fact]
        public void Update_WithValidData_UpdatesProductProperties()
        {
            var product = CreateProduct();
            var categoryId = Guid.NewGuid();
            product.Update("Wireless Mouse Pro", 399.99m, categoryId, "Updated description");
            Assert.Equal("Wireless Mouse Pro", product.Name);
            Assert.Equal(399.99m, product.Price);
            Assert.Equal(categoryId, product.CategoryId);
            Assert.Equal("Updated description", product.Description);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            var product = CreateProduct();
            Assert.Throws<ArgumentException>(() => product.Update("", 399.99m, Guid.NewGuid(), null));
        }

        [Fact]
        public void Update_WithNegativePrice_ThrowsArgumentOutOfRangeException()
        {
            var product = CreateProduct();
            Assert.Throws<ArgumentOutOfRangeException>(() => product.Update("Wireless Mouse Pro", -1m, Guid.NewGuid(), null));
        }

        [Fact]
        public void Update_WithEmptyCategoryId_ThrowsArgumentException()
        {
            var product = CreateProduct();
            Assert.Throws<ArgumentException>(() => product.Update("Wireless Mouse Pro", 399.99m, Guid.Empty, null));
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            var product = CreateProduct();
            product.Deactivate();
            Assert.False(product.IsActive);
        }

        [Fact]
        public void ApplyInventoryMovement_WithEntry_IncreasesStock()
        {
            var product = CreateProduct();
            product.ApplyInventoryMovement(true, 10);
            Assert.Equal(10, product.StockQuantity);
        }

        [Fact]
        public void ApplyInventoryMovement_WithExit_DecreasesStock()
        {
            var product = CreateProduct();
            product.ApplyInventoryMovement(true, 10);
            product.ApplyInventoryMovement(false, 4);
            Assert.Equal(6, product.StockQuantity);
        }

        [Fact]
        public void ApplyInventoryMovement_WithInsufficientStock_ThrowsInvalidOperationException()
        {
            var product = CreateProduct();
            product.ApplyInventoryMovement(true, 5);
            Assert.Throws<InvalidOperationException>(() => product.ApplyInventoryMovement(false, 6));
            Assert.Equal(5, product.StockQuantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ApplyInventoryMovement_WithInvalidQuantity_ThrowsArgumentOutOfRangeException(int quantity)
        {
            var product = CreateProduct();
            Assert.Throws<ArgumentOutOfRangeException>(() => product.ApplyInventoryMovement(true, quantity));
        }

        [Fact]
        public void ApplyInventoryMovement_WithStockOverflow_ThrowsOverflowException()
        {
            var product = CreateProduct();
            product.ApplyInventoryMovement(true, int.MaxValue);
            Assert.Throws<OverflowException>(() => product.ApplyInventoryMovement(true, 1));
            Assert.Equal(int.MaxValue, product.StockQuantity);
        }
    }
}
