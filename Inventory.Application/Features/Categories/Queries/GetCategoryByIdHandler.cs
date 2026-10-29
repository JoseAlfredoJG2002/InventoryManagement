using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;

namespace Inventory.Application.Features.Categories.Queries
{
    public sealed class GetCategoryByIdHandler
    {
        private readonly ICategoryQueryRepository _categoryQueryRepository;

        public GetCategoryByIdHandler(
            ICategoryQueryRepository categoryQueryRepository)
        {
            _categoryQueryRepository = categoryQueryRepository;
        }

        public async Task<CategoryResponse> HandleAsync(
            Guid id,
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

            return new CategoryResponse(
                category.Id,
                category.Name,
                category.Description);
        }
    }
}
