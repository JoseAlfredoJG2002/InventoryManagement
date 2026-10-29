using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(category => category.Name)
                .IsUnique();
            builder.Property(category => category.Description)
                .HasMaxLength(500);
            builder.HasMany(category => category.Products)
                .WithOne(product => product.Category)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
