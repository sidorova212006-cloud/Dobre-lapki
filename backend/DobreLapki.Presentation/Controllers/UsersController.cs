using DobreLapki.Domain.Common;
using DobreLapki.Domain.Services;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Contracts.Users;
using DobreLapki.Presentation.Mapping;
using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Controllers;

/// <summary>Пользователи: регистрация, вход, профиль и CRUD.</summary>
[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly PaginationOptions _pagination;

    /// <summary>Создаёт контроллер поверх доменного сервиса пользователей.</summary>
    public UsersController(IUserService service, IOptions<PaginationOptions> pagination)
    {
        _service = service;
        _pagination = pagination.Value;
    }

    /// <summary>Возвращает страницу пользователей.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAll(
        [FromQuery] UserQuery query, CancellationToken cancellationToken)
    {
        var (page, pageSize) = QueryParsing.NormalizePaging(_pagination, query.Page, query.PageSize);
        var result = await _service.GetPagedAsync(page, pageSize, cancellationToken);

        return Ok(new PagedResult<UserDto>(
            result.Items.Select(user => user.ToDto()).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount));
    }

    /// <summary>Возвращает пользователя по идентификатору.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(user.ToDto());
    }

    /// <summary>Регистрирует нового пользователя.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Register(
        [FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _service.RegisterAsync(request.ToDraft(), cancellationToken);
        var dto = user.ToDto();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Проверяет учётные данные. Не создаёт ресурс, поэтому возвращает 200.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _service.LoginAsync(request.ToDraft(), cancellationToken);
        return Ok(user.ToDto());
    }

    /// <summary>Полностью обновляет профиль пользователя.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Update(
        int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _service.UpdateAsync(id, request.ToDraft(), cancellationToken);
        return Ok(user.ToDto());
    }

    /// <summary>Удаляет пользователя.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
