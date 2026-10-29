using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class ProductQueryRepository : IProductQueryRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductQueryRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    product => product.Id == id,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .OrderBy(product => product.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
