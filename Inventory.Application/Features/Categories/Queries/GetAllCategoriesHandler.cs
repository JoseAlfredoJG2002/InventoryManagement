using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;

namespace Inventory.Application.Features.Categories.Queries
{
    public sealed class GetAllCategoriesHandler
    {
        private readonly ICategoryQueryRepository _categoryQueryRepository;

        public GetAllCategoriesHandler(
            ICategoryQueryRepository categoryQueryRepository)
        {
            _categoryQueryRepository = categoryQueryRepository;
        }

        public async Task<IReadOnlyList<CategoryResponse>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var categories = await _categoryQueryRepository.GetAllAsync(
                cancellationToken);

            return categories
                .Select(category => new CategoryResponse(
                    category.Id,
                    category.Name,
                    category.Description))
                .ToList();
        }
    }
}
