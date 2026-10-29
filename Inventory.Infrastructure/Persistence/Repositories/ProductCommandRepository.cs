using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class ProductCommandRepository : IProductCommandRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductCommandRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO Products
                (Id, SKU, Name, Description, Price, StockQuantity, CategoryId, IsActive)
            VALUES
                (@Id, @SKU, @Name, @Description, @Price, @StockQuantity, @CategoryId, @IsActive);
            """;

            await _dbContext.Database.GetDbConnection().ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    product.Id,
                    product.SKU,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.StockQuantity,
                    product.CategoryId,
                    product.IsActive
                }, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE Products
            SET Name = @Name,
                Description = @Description,
                Price = @Price,
                CategoryId = @CategoryId
            WHERE Id = @Id;
            """;

            var affectedRows = await _dbContext.Database.GetDbConnection().ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.CategoryId
                }, cancellationToken: cancellationToken));

            if (affectedRows == 0)
            {
                throw new KeyNotFoundException(
                    $"Product with ID '{product.Id}' was not found.");
            }
        }

        public async Task<bool> DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE Products
            SET IsActive = 0
            WHERE Id = @Id;
            """;

            var affectedRows = await _dbContext.Database.GetDbConnection().ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new { Id = id },
                    cancellationToken: cancellationToken));

            return affectedRows > 0;
        }
    }
}
