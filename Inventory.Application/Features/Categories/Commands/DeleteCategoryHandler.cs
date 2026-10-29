using Inventory.Application.Abstractions.Persistence;

namespace Inventory.Application.Features.Categories.Commands
{
    public sealed class DeleteCategoryHandler
    {
        private readonly ICategoryCommandRepository _categoryCommandRepository;

        public DeleteCategoryHandler(
            ICategoryCommandRepository categoryCommandRepository)
        {
            _categoryCommandRepository = categoryCommandRepository;
        }

        public async Task HandleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var deleted = await _categoryCommandRepository.DeleteAsync(
                id,
                cancellationToken);

            if (!deleted)
            {
                throw new KeyNotFoundException(
                    $"Category with ID '{id}' was not found.");
            }
        }
    }
}
