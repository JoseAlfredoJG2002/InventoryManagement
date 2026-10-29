namespace Inventory.Application.DTOs.Products;
public sealed record CreateProductRequest(
    string SKU,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId);