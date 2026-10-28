namespace Inventory.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public Guid CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsActive { get; private set; } = true;
        private Product() { }
        public Product(string sku, string name, decimal price, Guid categoryId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("Product SKU is required.", nameof(sku));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
            if (categoryId == Guid.Empty)
                throw new ArgumentException("A valid category is required.", nameof(categoryId));
            Id = Guid.NewGuid();
            SKU = sku.Trim();
            Name = name.Trim();
            Price = price;
            CategoryId = categoryId;
            Description = description;
            StockQuantity = 0;
        }
        public void Update(string name, decimal price, Guid categoryId, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
            if (categoryId == Guid.Empty)
                throw new ArgumentException("A valid category is required.", nameof(categoryId));
            Name = name.Trim();
            Price = price;
            CategoryId = categoryId;
            Description = description;
        }
        public void ApplyInventoryMovement(bool isEntry, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            if (!isEntry && quantity > StockQuantity)
                throw new InvalidOperationException("Insufficient stock.");
            StockQuantity = isEntry ? StockQuantity + quantity : StockQuantity - quantity;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
