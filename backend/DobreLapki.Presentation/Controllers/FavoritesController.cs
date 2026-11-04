using DobreLapki.Domain.Common;
using DobreLapki.Domain.Services;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Contracts.Favorites;
using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Controllers;

/// <summary>Избранные животные.</summary>
[ApiController]
[Route("api/v1/favorites")]
[Produces("application/json")]
public sealed class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _service;
    private readonly PaginationOptions _pagination;

    /// <summary>Создаёт контроллер поверх доменного сервиса избранного.</summary>
    public FavoritesController(IFavoriteService service, IOptions<PaginationOptions> pagination)
    {
        _service = service;
        _pagination = pagination.Value;
    }

    /// <summary>Возвращает страницу идентификаторов избранного.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<int>>> GetAll(
        [FromQuery] FavoriteQuery query, CancellationToken cancellationToken)
    {
        var (page, pageSize) = QueryParsing.NormalizePaging(_pagination, query.Page, query.PageSize);
        var result = await _service.GetPagedAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>Добавляет животное в избранное.</summary>
    [HttpPost("{animalId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(int animalId, CancellationToken cancellationToken)
    {
        await _service.AddAsync(animalId, cancellationToken);
        return Created($"/api/v1/favorites/{animalId}", animalId);
    }

    /// <summary>Удаляет животное из избранного.</summary>
    [HttpDelete("{animalId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(int animalId, CancellationToken cancellationToken)
    {
        await _service.RemoveAsync(animalId, cancellationToken);
        return NoContent();
    }
}
