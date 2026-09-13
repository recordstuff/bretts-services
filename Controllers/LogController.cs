using bretts_services.Models.ViewModels;
using System.Text.Json;

namespace bretts_services.Controllers;

/// <summary>Provides administrative CRUD and search access to application log entries.</summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;

    /// <summary>Initializes a new instance of the <see cref="LogController"/> class.</summary>
    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>Gets a filtered page of application log entries.</summary>
    /// <remarks>
    /// Free text is matched across the rendered message, template, exception, and complete event JSON.
    /// Every structured attribute condition must match. Numeric comparison operators ignore values that
    /// cannot be converted to numbers.
    /// </remarks>
    /// <param name="searchParameters">Paging, sorting, time, level, text, and structured attribute filters.</param>
    /// <returns>A page of matching log entries.</returns>
    [HttpPost("logs")]
    [ProducesResponseType(typeof(PaginationResult<LogSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logs(LogSearchParameters searchParameters)
    {
        var invalidFilter = searchParameters.AttributeFilters.Any(filter =>
            string.IsNullOrWhiteSpace(filter.Attribute)
            || (filter.Operator != LogFilterOperator.Exists
                && filter.Operator != LogFilterOperator.DoesNotExist
                && string.IsNullOrWhiteSpace(filter.Value)));

        if (invalidFilter)
        {
            return BadRequest("Every attribute filter requires an attribute, and comparison operators require a value.");
        }

        if (searchParameters.From > searchParameters.To)
        {
            return BadRequest("From must be earlier than or equal to To.");
        }

        return Ok(await _logService.GetLogs(searchParameters));
    }

    /// <summary>Gets the distinct structured attributes currently present in stored events.</summary>
    /// <returns>Attribute names sorted alphabetically.</returns>
    [HttpGet("attributes")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Attributes()
    {
        return Ok(await _logService.GetAttributes());
    }

    /// <summary>Gets one application log entry.</summary>
    [HttpGet("log/{guid}")]
    [ProducesResponseType(typeof(LogDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Log(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return BadRequest("Log identifier cannot be empty.");
        }

        var log = await _logService.GetLog(guid);
        if (log is null)
        {
            return NotFound();
        }

        return Ok(log);
    }

    /// <summary>Creates an application log entry.</summary>
    [HttpPost("insert")]
    [ProducesResponseType(typeof(LogDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Insert(LogNew log)
    {
        if (!IsValidLogEvent(log.LogEvent))
        {
            return BadRequest("Log event must contain valid JSON when provided.");
        }

        var insertedLog = await _logService.InsertLog(log);
        return CreatedAtAction(nameof(Log), new { guid = insertedLog.Guid }, insertedLog);
    }

    /// <summary>Updates an application log entry.</summary>
    [HttpPost("update")]
    [ProducesResponseType(typeof(LogDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(LogDetail log)
    {
        if (log.Guid == Guid.Empty)
        {
            return BadRequest("Log identifier cannot be empty.");
        }

        if (!IsValidLogEvent(log.LogEvent))
        {
            return BadRequest("Log event must contain valid JSON when provided.");
        }

        var updatedLog = await _logService.UpdateLog(log);
        if (updatedLog is null)
        {
            return NotFound();
        }

        return Ok(updatedLog);
    }

    /// <summary>Deletes an application log entry.</summary>
    [HttpDelete("delete/{guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return BadRequest("Log identifier cannot be empty.");
        }

        if (!await _logService.DeleteLog(guid))
        {
            return NotFound();
        }

        return Ok(true);
    }

    private static bool IsValidLogEvent(string? logEvent)
    {
        if (string.IsNullOrWhiteSpace(logEvent))
        {
            return true;
        }

        try
        {
            using var document = JsonDocument.Parse(logEvent);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
