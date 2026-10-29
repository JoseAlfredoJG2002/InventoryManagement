using Inventory.Application.DTOs.Products;
using Inventory.Application.Features.Products.Commands;
using Inventory.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly CreateProductHandler _createProductHandler;
        private readonly UpdateProductHandler _updateProductHandler;
        private readonly DeactivateProductHandler _deactivateProductHandler;
        private readonly GetAllProductsHandler _getAllProductsHandler;
        private readonly GetProductByIdHandler _getProductByIdHandler;

        public ProductsController(
            CreateProductHandler createProductHandler,
            UpdateProductHandler updateProductHandler,
            DeactivateProductHandler deactivateProductHandler,
            GetAllProductsHandler getAllProductsHandler,
            GetProductByIdHandler getProductByIdHandler)
        {
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
            _deactivateProductHandler = deactivateProductHandler;
            _getAllProductsHandler = getAllProductsHandler;
            _getProductByIdHandler = getProductByIdHandler;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ProductResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(
            CancellationToken cancellationToken)
        {
            var products = await _getAllProductsHandler.HandleAsync(cancellationToken);

            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponse>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var product = await _getProductByIdHandler.HandleAsync(id, cancellationToken);

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponse>> Create(
            CreateProductRequest request,
            CancellationToken cancellationToken)
        {
            var product = await _createProductHandler.HandleAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponse>> Update(
            Guid id,
            UpdateProductRequest request,
            CancellationToken cancellationToken)
        {
            var product = await _updateProductHandler.HandleAsync(id, request, cancellationToken);

            return Ok(product);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _deactivateProductHandler.HandleAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
