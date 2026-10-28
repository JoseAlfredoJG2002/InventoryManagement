namespace Inventory.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public ICollection<Product> Products { get; private set; } = new List<Product>();
        private Category() { }
        public Category(string name, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));
            Id = Guid.NewGuid();
            Name = name.Trim();
            Description = description;
        }
        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));
            Name = name.Trim();
            Description = description;
        }
    }
}
