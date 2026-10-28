namespace Inventory.Domain.Entities
{
    public class InventoryMovement
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public InventoryMovementType Type { get; private set; }
        public int Quantity { get; private set; }
        public DateTime OccurredAtUtc { get; private set; }
        public string? Reference { get; private set; }
        private InventoryMovement() { }
        public InventoryMovement(Guid productId, InventoryMovementType type, int quantity, string? reference = null)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("A valid product is required.", nameof(productId));
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), "Invalid movement type.");
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            Id = Guid.NewGuid();
            ProductId = productId;
            Type = type;
            Quantity = quantity;
            OccurredAtUtc = DateTime.UtcNow;
            Reference = reference;
        }
    }
}
