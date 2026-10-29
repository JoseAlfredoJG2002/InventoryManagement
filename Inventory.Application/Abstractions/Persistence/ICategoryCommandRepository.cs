using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence
{
    public interface ICategoryCommandRepository
    {
        Task AddAsync( Category category, CancellationToken cancellationToken = default);

        Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
