using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", table =>
            {
                table.HasCheckConstraint("CK_Products_Price", "[Price] >= 0");
                table.HasCheckConstraint("CK_Products_StockQuantity", "[StockQuantity] >= 0");
            });
            builder.HasKey(product => product.Id);
            builder.Property(product => product.SKU)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasIndex(product => product.SKU)
                .IsUnique();
            builder.Property(product => product.Name)
                .HasMaxLength(150)
                .IsRequired();
            builder.Property(product => product.Description)
                .HasMaxLength(1000);
            builder.Property(product => product.Price)
                .HasPrecision(18, 2);
            builder.Property(product => product.StockQuantity)
                .IsRequired();
            builder.Property(product => product.IsActive)
                .IsRequired();
            builder.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
