using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Configurations
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
    {
        public void Configure(EntityTypeBuilder<InventoryMovement> builder)
        {
            builder.ToTable("InventoryMovements", table =>
            {
                table.HasCheckConstraint("CK_InventoryMovements_Quantity", "[Quantity] > 0");
                table.HasCheckConstraint("CK_InventoryMovements_Type", "[Type] IN (1, 2)");
            });
            builder.HasKey(movement => movement.Id);
            builder.Property(movement => movement.Type)
                .HasConversion<int>()
                .IsRequired();
            builder.Property(movement => movement.Quantity)
                .IsRequired();
            builder.Property(movement => movement.OccurredAtUtc)
                .IsRequired();
            builder.Property(movement => movement.Reference)
                .HasMaxLength(200);
            builder.HasIndex(movement => new { movement.ProductId, movement.OccurredAtUtc });
            builder.HasOne(movement => movement.Product)
                .WithMany()
                .HasForeignKey(movement => movement.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
