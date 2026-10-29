namespace Inventory.Application.DTOs.Products;

public sealed record ProductResponse(
    Guid Id,
    string SKU,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId,
    bool IsActive);