using Inventory.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.UnitTests.Domain
{
    public class InventoryMovementTests
    {
        [Fact]
        public void Constructor_WithValidData_CreatesInventoryMovement()
        {
            var productId = Guid.NewGuid();
            var movement = new InventoryMovement(
                productId,
                InventoryMovementType.Entry,
                10,
                "Initial stock");

            Assert.NotEqual(Guid.Empty, movement.Id);
            Assert.Equal(productId, movement.ProductId);
            Assert.Equal(InventoryMovementType.Entry, movement.Type);
            Assert.Equal(10, movement.Quantity);
            Assert.Equal("Initial stock", movement.Reference);
            Assert.Equal(DateTimeKind.Utc, movement.OccurredAtUtc.Kind);
        }

        [Fact]
        public void Constructor_WithEmptyProductId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new InventoryMovement(
                    Guid.Empty,
                    InventoryMovementType.Entry,
                    10));
        }

        [Fact]
        public void Constructor_WithInvalidType_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryMovement(
                    Guid.NewGuid(),
                    (InventoryMovementType)999,
                    10));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_WithInvalidQuantity_ThrowsArgumentOutOfRangeException(int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryMovement(
                    Guid.NewGuid(),
                    InventoryMovementType.Entry,
                    quantity));
        }

        [Fact]
        public void Constructor_WithoutReference_CreatesMovementWithNullReference()
        {
            var movement = new InventoryMovement(
                Guid.NewGuid(),
                InventoryMovementType.Exit,
                5);

            Assert.Null(movement.Reference);
        }
    }
}
