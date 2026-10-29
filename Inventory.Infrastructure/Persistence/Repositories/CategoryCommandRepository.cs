using Dapper;
using Inventory.Application.Abstractions.Persistence;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class CategoryCommandRepository : ICategoryCommandRepository
    {
        private readonly InventoryDbContext _dbContext;

        public CategoryCommandRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            INSERT INTO Categories (Id, Name, Description)
            VALUES (@Id, @Name, @Description);
            """;

            await _dbContext.Database.GetDbConnection().ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    category.Id,
                    category.Name,
                    category.Description
                }, cancellationToken: cancellationToken));
        }

        public async Task UpdateAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            UPDATE Categories
            SET Name = @Name,
                Description = @Description
            WHERE Id = @Id;
            """;

            var affectedRows = await _dbContext.Database.GetDbConnection().ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    category.Id,
                    category.Name,
                    category.Description
                }, cancellationToken: cancellationToken));

            if (affectedRows == 0)
            {
                throw new KeyNotFoundException(
                    $"Category with ID '{category.Id}' was not found.");
            }
        }

        public async Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
            DELETE FROM Categories
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
