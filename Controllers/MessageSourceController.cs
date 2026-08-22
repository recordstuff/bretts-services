using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Controllers;

/// <summary>
/// Provides administrative read-only access to message sources captured by JunkEmailCleaner.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class MessageSourceController : ControllerBase
{
    private const int MaximumPageSize = 200;
    private readonly JunkEmailCleanerContext _junkEmailCleanerContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSourceController"/> class.
    /// </summary>
    /// <param name="junkEmailCleanerContext">The read-only JunkEmailCleaner database context.</param>
    public MessageSourceController(JunkEmailCleanerContext junkEmailCleanerContext)
    {
        _junkEmailCleanerContext = junkEmailCleanerContext;
    }

    /// <summary>
    /// Gets a page of stored message-source summaries, newest first.
    /// </summary>
    /// <param name="page">The one-based page number to return.</param>
    /// <param name="pageSize">The number of records to return, from 1 through 200.</param>
    /// <param name="cancellationToken">Signals that the request has been cancelled.</param>
    /// <returns>A page of message-source summaries without the original email text.</returns>
    /// <response code="200">Returns the requested page of message-source summaries.</response>
    /// <response code="400">The page or page size is outside its allowed range.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("message-sources")]
    [ProducesResponseType(typeof(PaginationResult<MessageSourceSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MessageSources(int page = 1, int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest("Page must be at least 1.");
        }

        if (pageSize < 1 || pageSize > MaximumPageSize)
        {
            return BadRequest($"Page size must be between 1 and {MaximumPageSize}.");
        }

        var query = _junkEmailCleanerContext.MessageSources;
        var itemCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(messageSource => messageSource.MessageSourceId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(messageSource => new MessageSourceSummary
            {
                MessageSourceId = messageSource.MessageSourceId,
                BlockedSenderName = messageSource.BlockedSenderName,
                GraphMessageId = messageSource.GraphMessageId,
            })
            .ToListAsync(cancellationToken);

        var result = new PaginationResult<MessageSourceSummary>
        {
            Page = page,
            PageCount = (int)Math.Ceiling((double)itemCount / pageSize),
            ItemCount = itemCount,
            Items = items,
        };

        return Ok(result);
    }

    /// <summary>
    /// Gets one stored message source including its complete original email text.
    /// </summary>
    /// <param name="messageSourceId">The database identifier of the stored message source.</param>
    /// <param name="cancellationToken">Signals that the request has been cancelled.</param>
    /// <returns>The matching stored message source.</returns>
    /// <response code="200">Returns the matching stored message source.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No message source has the supplied identifier.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpGet("message-source/{messageSourceId:long}")]
    [ProducesResponseType(typeof(MessageSourceDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MessageSource(long messageSourceId,
        CancellationToken cancellationToken = default)
    {
        var messageSource = await _junkEmailCleanerContext.MessageSources
            .Where(candidate => candidate.MessageSourceId == messageSourceId)
            .Select(candidate => new MessageSourceDetail
            {
                MessageSourceId = candidate.MessageSourceId,
                BlockedSenderName = candidate.BlockedSenderName,
                ViewMessageSourceText = candidate.ViewMessageSourceText,
                GraphMessageId = candidate.GraphMessageId,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (messageSource is null)
        {
            return NotFound();
        }

        return Ok(messageSource);
    }
}
