using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence
{
    public interface IProductCommandRepository
    {
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
