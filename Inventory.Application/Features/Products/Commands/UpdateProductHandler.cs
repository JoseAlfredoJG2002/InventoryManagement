using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;

namespace Inventory.Application.Features.Products.Commands
{
    public sealed class UpdateProductHandler
    {
        private readonly IProductQueryRepository _productQueryRepository;
        private readonly IProductCommandRepository _productCommandRepository;
        private readonly ICategoryQueryRepository _categoryQueryRepository;

        public UpdateProductHandler(
            IProductQueryRepository productQueryRepository,
            IProductCommandRepository productCommandRepository,
            ICategoryQueryRepository categoryQueryRepository)
        {
            _productQueryRepository = productQueryRepository;
            _productCommandRepository = productCommandRepository;
            _categoryQueryRepository = categoryQueryRepository;
        }

        public async Task<ProductResponse> HandleAsync(
            Guid id,
            UpdateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var product = await _productQueryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID '{id}' was not found.");
            }

            var category = await _categoryQueryRepository.GetByIdAsync(
                request.CategoryId,
                cancellationToken);

            if (category is null)
            {
                throw new KeyNotFoundException(
                    $"Category with ID '{request.CategoryId}' was not found.");
            }

            product.Update(
                request.Name,
                request.Price,
                request.CategoryId,
                request.Description);

            await _productCommandRepository.UpdateAsync(
                product,
                cancellationToken);

            return new ProductResponse(
                product.Id,
                product.SKU,
                product.Name,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.CategoryId,
                product.IsActive);
        }
    }
}
