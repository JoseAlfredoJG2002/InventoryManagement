using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;
using Inventory.Domain.Entities;

namespace Inventory.Application.Features.Products.Commands
{
    public sealed class CreateProductHandler
    {
        private readonly IProductCommandRepository _productCommandRepository;
        private readonly ICategoryQueryRepository _categoryQueryRepository;

        public CreateProductHandler(
            IProductCommandRepository productCommandRepository,
            ICategoryQueryRepository categoryQueryRepository)
        {
            _productCommandRepository = productCommandRepository;
            _categoryQueryRepository = categoryQueryRepository;
        }

        public async Task<ProductResponse> HandleAsync(
            CreateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryQueryRepository.GetByIdAsync(
                request.CategoryId,
                cancellationToken);

            if (category is null)
            {
                throw new KeyNotFoundException(
                    $"Category with ID '{request.CategoryId}' was not found.");
            }

            var product = new Product(
                request.SKU,
                request.Name,
                request.Price,
                request.CategoryId,
                request.Description);

            await _productCommandRepository.AddAsync(
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
