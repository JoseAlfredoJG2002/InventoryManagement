using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryMovementCommandRepository : IInventoryMovementCommandRepository
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryMovementCommandRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task RegisterAsync(
            Product product,
            InventoryMovement movement,
            CancellationToken cancellationToken = default)
        {
            var expectedStock = movement.Type == InventoryMovementType.Entry
                ? product.StockQuantity - movement.Quantity
                : product.StockQuantity + movement.Quantity;

            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var dbTransaction = transaction.GetDbTransaction();
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                const string updateStockSql = """
                UPDATE Products
                SET StockQuantity = @NewStock
                WHERE Id = @ProductId
                    AND StockQuantity = @ExpectedStock
                    AND IsActive = 1;
                """;

                var affectedRows = await connection.ExecuteAsync(
                    new CommandDefinition(
                        updateStockSql,
                        new
                        {
                            ProductId = product.Id,
                            NewStock = product.StockQuantity,
                            ExpectedStock = expectedStock
                        },
                        transaction: dbTransaction,
                        cancellationToken: cancellationToken));

                if (affectedRows != 1)
                {
                    throw new InvalidOperationException(
                        "The product stock changed or the product is inactive. Retry the operation.");
                }

                const string insertMovementSql = """
                INSERT INTO InventoryMovements
                    (Id, ProductId, Type, Quantity, OccurredAtUtc, Reference)
                VALUES
                    (@Id, @ProductId, @Type, @Quantity, @OccurredAtUtc, @Reference);
                """;

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        insertMovementSql,
                        new
                        {
                            movement.Id,
                            movement.ProductId,
                            Type = (int)movement.Type,
                            movement.Quantity,
                            movement.OccurredAtUtc,
                            movement.Reference
                        },
                        transaction: dbTransaction,
                        cancellationToken: cancellationToken));

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }
    }
}
