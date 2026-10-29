using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;

namespace Inventory.Application.Features.Categories.Commands
{
    public sealed class UpdateCategoryHandler
    {
        private readonly ICategoryQueryRepository _categoryQueryRepository;
        private readonly ICategoryCommandRepository _categoryCommandRepository;

        public UpdateCategoryHandler(
            ICategoryQueryRepository categoryQueryRepository,
            ICategoryCommandRepository categoryCommandRepository)
        {
            _categoryQueryRepository = categoryQueryRepository;
            _categoryCommandRepository = categoryCommandRepository;
        }

        public async Task<CategoryResponse> HandleAsync(
            Guid id,
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryQueryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (category is null)
            {
                throw new KeyNotFoundException(
                    $"Category with ID '{id}' was not found.");
            }

            category.Update(request.Name, request.Description);

            await _categoryCommandRepository.UpdateAsync(
                category,
                cancellationToken);

            return new CategoryResponse(
                category.Id,
                category.Name,
                category.Description);
        }
    }
}
