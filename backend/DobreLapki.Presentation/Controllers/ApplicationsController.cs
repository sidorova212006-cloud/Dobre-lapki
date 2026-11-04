using DobreLapki.Domain.Common;
using DobreLapki.Domain.Services;
using DobreLapki.Presentation.Contracts.Applications;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Mapping;
using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Controllers;

/// <summary>CRUD заявок на усыновление.</summary>
[ApiController]
[Route("api/v1/applications")]
[Produces("application/json")]
public sealed class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;
    private readonly PaginationOptions _pagination;

    /// <summary>Создаёт контроллер поверх доменного сервиса заявок.</summary>
    public ApplicationsController(
        IApplicationService service, IOptions<PaginationOptions> pagination)
    {
        _service = service;
        _pagination = pagination.Value;
    }

    /// <summary>Возвращает страницу заявок (сначала новые).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ApplicationDto>>> GetAll(
        [FromQuery] ApplicationQuery query, CancellationToken cancellationToken)
    {
        var (page, pageSize) = QueryParsing.NormalizePaging(_pagination, query.Page, query.PageSize);
        var result = await _service.GetPagedAsync(page, pageSize, cancellationToken);

        return Ok(new PagedResult<ApplicationDto>(
            result.Items.Select(application => application.ToDto()).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount));
    }

    /// <summary>Возвращает заявку по идентификатору.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(
        int id, CancellationToken cancellationToken)
    {
        var application = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(application.ToDto());
    }

    /// <summary>Создаёт заявку на усыновление.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationDto>> Create(
        [FromBody] CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await _service.CreateAsync(request.ToDraft(), cancellationToken);
        var dto = application.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Изменяет статус заявки.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationDto>> Update(
        int id, [FromBody] UpdateApplicationRequest request, CancellationToken cancellationToken)
    {
        var application = await _service.UpdateAsync(
            id, new Domain.Models.ApplicationUpdateDraft(request.Status), cancellationToken);
        return Ok(application.ToDto());
    }

    /// <summary>Удаляет заявку.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
