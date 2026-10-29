namespace Inventory.Application.DTOs.Categories;
    public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string? Description);
