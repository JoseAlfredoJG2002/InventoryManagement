using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Persistence
{
    public interface ICategoryQueryRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
