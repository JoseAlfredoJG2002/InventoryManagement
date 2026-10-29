using Inventory.Domain.Entities;

namespace Inventory.Application.DTOs.Inventory;
    public sealed record InventoryMovementResponse(
    Guid Id,
    Guid ProductId,
    InventoryMovementType Type,
    int Quantity,
    DateTime OccurredAtUtc,
    string? Reference,
    int CurrentStock);