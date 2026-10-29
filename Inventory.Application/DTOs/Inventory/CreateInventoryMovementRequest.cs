using Inventory.Domain.Entities;

namespace Inventory.Application.DTOs.Inventory;
    public sealed record CreateInventoryMovementRequest(
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    string? Reference);