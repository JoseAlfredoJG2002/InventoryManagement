using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence
{
    public interface IProductQueryRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
