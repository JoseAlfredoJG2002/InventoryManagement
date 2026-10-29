namespace Inventory.Application.DTOs.Products;

    public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId);
