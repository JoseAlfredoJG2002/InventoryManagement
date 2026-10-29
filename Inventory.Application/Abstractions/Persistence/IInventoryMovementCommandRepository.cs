using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence
{
    public interface IInventoryMovementCommandRepository
    {
        Task RegisterAsync(Product product, InventoryMovement movement, CancellationToken cancellationToken = default);
    }
}
