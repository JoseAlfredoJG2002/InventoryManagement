using Inventory.Application.Abstractions.Persistence;

namespace Inventory.Application.Features.Products.Commands
{
    public sealed class DeactivateProductHandler
    {
        private readonly IProductCommandRepository _productCommandRepository;

        public DeactivateProductHandler(
            IProductCommandRepository productCommandRepository)
        {
            _productCommandRepository = productCommandRepository;
        }

        public async Task HandleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var deactivated = await _productCommandRepository.DeactivateAsync(
                id,
                cancellationToken);

            if (!deactivated)
            {
                throw new KeyNotFoundException(
                    $"Product with ID '{id}' was not found.");
            }
        }
    }
}
