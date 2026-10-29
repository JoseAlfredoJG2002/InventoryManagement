using Inventory.Application.DTOs.Inventory;
using Inventory.Application.Features.Inventory.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers

{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class InventoryMovementsController : ControllerBase
    {
        private readonly RegisterInventoryMovementHandler _registerInventoryMovementHandler;

        public InventoryMovementsController(
            RegisterInventoryMovementHandler registerInventoryMovementHandler)
        {
            _registerInventoryMovementHandler = registerInventoryMovementHandler;
        }

        [HttpPost]
        [ProducesResponseType(typeof(InventoryMovementResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryMovementResponse>> Register(
            CreateInventoryMovementRequest request,
            CancellationToken cancellationToken)
        {
            var movement = await _registerInventoryMovementHandler.HandleAsync(
                request,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, movement);
        }
    }
}
