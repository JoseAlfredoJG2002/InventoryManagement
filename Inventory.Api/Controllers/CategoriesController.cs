using Inventory.Application.DTOs.Categories;
using Inventory.Application.Features.Categories.Commands;
using Inventory.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;
    private readonly UpdateCategoryHandler _updateCategoryHandler;
    private readonly DeleteCategoryHandler _deleteCategoryHandler;
    private readonly GetAllCategoriesHandler _getAllCategoriesHandler;
    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;

    public CategoriesController(
        CreateCategoryHandler createCategoryHandler,
        UpdateCategoryHandler updateCategoryHandler,
        DeleteCategoryHandler deleteCategoryHandler,
        GetAllCategoriesHandler getAllCategoriesHandler,
        GetCategoryByIdHandler getCategoryByIdHandler)
    {
        _createCategoryHandler = createCategoryHandler;
        _updateCategoryHandler = updateCategoryHandler;
        _deleteCategoryHandler = deleteCategoryHandler;
        _getAllCategoriesHandler = getAllCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var categories = await _getAllCategoriesHandler.HandleAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _getCategoryByIdHandler.HandleAsync(id, cancellationToken);
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponse>> Create(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _createCategoryHandler.HandleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponse>> Update(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _updateCategoryHandler.HandleAsync(id, request, cancellationToken);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _deleteCategoryHandler.HandleAsync(id, cancellationToken);
        return NoContent();
    }
}
