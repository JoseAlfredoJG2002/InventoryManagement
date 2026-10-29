using Inventory.Application.Abstractions.Persistence;
using Inventory.Application.DTOs.Inventory;
using Inventory.Domain.Entities;

namespace Inventory.Application.Features.Inventory.Commands
{
    public sealed class RegisterInventoryMovementHandler
    {
        private readonly IProductQueryRepository _productQueryRepository;
        private readonly IInventoryMovementCommandRepository _movementCommandRepository;

        public RegisterInventoryMovementHandler(
            IProductQueryRepository productQueryRepository,
            IInventoryMovementCommandRepository movementCommandRepository)
        {
            _productQueryRepository = productQueryRepository;
            _movementCommandRepository = movementCommandRepository;
        }

        public async Task<InventoryMovementResponse> HandleAsync(
            CreateInventoryMovementRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(request.Type))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request.Type),
                    "The inventory movement type is invalid.");
            }

            if (request.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(request.Quantity),
                    "Quantity must be greater than zero.");
            }

            var product = await _productQueryRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken);

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product with ID '{request.ProductId}' was not found.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    "Inventory movements cannot be registered for inactive products.");
            }

            var isEntry = request.Type == InventoryMovementType.Entry;

            product.ApplyInventoryMovement(isEntry, request.Quantity);

            var movement = new InventoryMovement(
                product.Id,
                request.Type,
                request.Quantity,
                request.Reference);

            await _movementCommandRepository.RegisterAsync(
                product,
                movement,
                cancellationToken);

            return new InventoryMovementResponse(
                movement.Id,
                movement.ProductId,
                movement.Type,
                movement.Quantity,
                movement.OccurredAtUtc,
                movement.Reference,
                product.StockQuantity);
        }
    }
}
