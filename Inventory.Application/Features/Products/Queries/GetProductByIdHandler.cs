using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;

namespace Inventory.Application.Features.Products.Queries
{
    public sealed class GetProductByIdHandler
    {
        private readonly IProductQueryRepository _productQueryRepository;

        public GetProductByIdHandler(IProductQueryRepository productQueryRepository)
        {
            _productQueryRepository = productQueryRepository;
        }

        public async Task<ProductResponse> HandleAsync(
            Guid id,
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
