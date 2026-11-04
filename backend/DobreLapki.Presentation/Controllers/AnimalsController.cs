using DobreLapki.Domain.Common;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Filters;
using DobreLapki.Domain.Services;
using DobreLapki.Presentation.Contracts.Animals;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Mapping;
using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Controllers;

/// <summary>CRUD животных приюта.</summary>
[ApiController]
[Route("api/v1/animals")]
[Produces("application/json")]
public sealed class AnimalsController : ControllerBase
{
    private readonly IAnimalService _service;
    private readonly PaginationOptions _pagination;

    /// <summary>Создаёт контроллер поверх доменного сервиса животных.</summary>
    public AnimalsController(IAnimalService service, IOptions<PaginationOptions> pagination)
    {
        _service = service;
        _pagination = pagination.Value;
    }

    /// <summary>Возвращает страницу каталога с фильтрами и сортировкой.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AnimalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<AnimalDto>>> GetAll(
        [FromQuery] AnimalQuery query, CancellationToken cancellationToken)
    {
        var filter = new AnimalFilter(
            QueryParsing.ParseEnumList<AnimalType>(query.Types, "types"),
            QueryParsing.ParseEnumList<AnimalGender>(query.Genders, "genders"),
            QueryParsing.ParseEnumList<AgeBucket>(query.Ages, "ages"),
            query.Search,
            QueryParsing.ParseEnum<AnimalSort>(query.Sort, AnimalSort.Newest, "sort"));

        var (page, pageSize) = QueryParsing.NormalizePaging(_pagination, query.Page, query.PageSize);
        var result = await _service.GetPagedAsync(filter, page, pageSize, cancellationToken);

        return Ok(new PagedResult<AnimalDto>(
            result.Items.Select(animal => animal.ToDto()).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount));
    }

    /// <summary>Возвращает популярных (самых новых) животных.</summary>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(IReadOnlyList<AnimalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AnimalDto>>> GetPopular(
        [FromQuery] PopularAnimalsQuery query, CancellationToken cancellationToken)
    {
        var animals = await _service.GetPopularAsync(query.Count, cancellationToken);
        return Ok(animals.Select(animal => animal.ToDto()).ToList());
    }

    /// <summary>Возвращает животное по идентификатору.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnimalDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var animal = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(animal.ToDto());
    }

    /// <summary>Создаёт животное.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AnimalDto>> Create(
        [FromBody] CreateAnimalRequest request, CancellationToken cancellationToken)
    {
        var animal = await _service.CreateAsync(request.ToDraft(), cancellationToken);
        var dto = animal.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Полностью обновляет животное.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AnimalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AnimalDto>> Update(
        int id, [FromBody] UpdateAnimalRequest request, CancellationToken cancellationToken)
    {
        var animal = await _service.UpdateAsync(id, request.ToDraft(), cancellationToken);
        return Ok(animal.ToDto());
    }

    /// <summary>Удаляет животное.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
