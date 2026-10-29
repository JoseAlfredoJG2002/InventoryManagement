using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Categories;
using Inventory.Domain.Entities;

namespace Inventory.Application.Features.Categories.Commands
{
    public sealed class CreateCategoryHandler
    {
        private readonly ICategoryCommandRepository _categoryCommandRepository;

        public CreateCategoryHandler(
            ICategoryCommandRepository categoryCommandRepository)
        {
            _categoryCommandRepository = categoryCommandRepository;
        }

        public async Task<CategoryResponse> HandleAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var category = new Category(
                request.Name,
                request.Description);

            await _categoryCommandRepository.AddAsync(
                category,
                cancellationToken);

            return new CategoryResponse(
                category.Id,
                category.Name,
                category.Description);
        }
    }
}
