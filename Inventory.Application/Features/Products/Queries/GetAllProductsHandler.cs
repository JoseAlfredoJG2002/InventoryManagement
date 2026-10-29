using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Products;

namespace Inventory.Application.Features.Products.Queries
{
    public sealed class GetAllProductsHandler
    {
        private readonly IProductQueryRepository _productQueryRepository;

        public GetAllProductsHandler(IProductQueryRepository productQueryRepository)
        {
            _productQueryRepository = productQueryRepository;
        }

        public async Task<IReadOnlyList<ProductResponse>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var products = await _productQueryRepository.GetAllAsync(cancellationToken);

            return products
                .Select(product => new ProductResponse(
                    product.Id,
                    product.SKU,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.StockQuantity,
                    product.CategoryId,
                    product.IsActive))
                .ToList();
        }
    }
}
