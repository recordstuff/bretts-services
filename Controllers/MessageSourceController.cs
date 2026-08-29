using bretts_services.Models.Entities;
using bretts_services.Models.ViewModels;

namespace bretts_services.Controllers;

/// <summary>
/// Provides administrative access to message sources captured by JunkEmailCleaner.
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
    /// <param name="junkEmailCleanerContext">The JunkEmailCleaner database context.</param>
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

        var query = _junkEmailCleanerContext.MessageSources
            .AsNoTracking();
        var itemCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(messageSource => messageSource.MessageSourceId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(messageSource => new MessageSourceSummary
            {
                MessageSourceId = messageSource.MessageSourceId,
                BlockedSenderName = messageSource.BlockedSenderName,
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
                MessageSource = candidate.MessageSource,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (messageSource is null)
        {
            return NotFound();
        }

        return Ok(messageSource);
    }

    /// <summary>
    /// Stores a new message source.
    /// </summary>
    /// <param name="newMessageSource">The message source to store.</param>
    /// <param name="cancellationToken">Signals that the request has been cancelled.</param>
    /// <returns>The stored message source including its generated database identifier.</returns>
    /// <response code="201">The message source was stored successfully.</response>
    /// <response code="400">The blocked sender name is missing.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="409">The complete blocked sender name is already stored.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("insert")]
    [ProducesResponseType(typeof(MessageSourceDetail), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Insert(MessageSourceNew newMessageSource,
        CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateBlockedSenderName(newMessageSource.BlockedSenderName);

        if (validationResult != null)
        {
            return validationResult;
        }

        if (await BlockedSenderNameExists(newMessageSource.BlockedSenderName, null, cancellationToken))
        {
            return Conflict();
        }

        var messageSource = new StoredMessageSource
        {
            BlockedSenderName = newMessageSource.BlockedSenderName,
            MessageSource = newMessageSource.MessageSource,
        };

        _junkEmailCleanerContext.MessageSources.Add(messageSource);
        await _junkEmailCleanerContext.SaveChangesAsync(cancellationToken);

        var detail = CreateDetail(messageSource);

        return CreatedAtAction(nameof(MessageSource), new { messageSourceId = messageSource.MessageSourceId }, detail);
    }

    /// <summary>
    /// Updates an existing message source.
    /// </summary>
    /// <param name="messageSourceDetail">The complete message source values to store.</param>
    /// <param name="cancellationToken">Signals that the request has been cancelled.</param>
    /// <returns>The updated message source.</returns>
    /// <response code="200">The message source was updated successfully.</response>
    /// <response code="400">The database identifier or blocked sender name is invalid.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No message source has the supplied database identifier.</response>
    /// <response code="409">Another message source already has the complete blocked sender name.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(MessageSourceDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(MessageSourceDetail messageSourceDetail,
        CancellationToken cancellationToken = default)
    {
        if (messageSourceDetail.MessageSourceId < 1)
        {
            return BadRequest("Message source ID must be at least 1.");
        }

        var validationResult = ValidateBlockedSenderName(messageSourceDetail.BlockedSenderName);

        if (validationResult != null)
        {
            return validationResult;
        }

        if (await BlockedSenderNameExists(messageSourceDetail.BlockedSenderName,
            messageSourceDetail.MessageSourceId, cancellationToken))
        {
            return Conflict();
        }

        var messageSource = await _junkEmailCleanerContext.MessageSources
            .SingleOrDefaultAsync(candidate => candidate.MessageSourceId == messageSourceDetail.MessageSourceId,
                cancellationToken);

        if (messageSource is null)
        {
            return NotFound();
        }

        messageSource.BlockedSenderName = messageSourceDetail.BlockedSenderName;
        messageSource.MessageSource = messageSourceDetail.MessageSource;

        await _junkEmailCleanerContext.SaveChangesAsync(cancellationToken);

        return Ok(CreateDetail(messageSource));
    }

    /// <summary>
    /// Deletes a stored message source.
    /// </summary>
    /// <param name="messageSourceId">The database identifier of the stored message source.</param>
    /// <param name="cancellationToken">Signals that the request has been cancelled.</param>
    /// <returns><see langword="true"/> when the message source is deleted.</returns>
    /// <response code="200">The message source was deleted successfully.</response>
    /// <response code="400">The database identifier is invalid.</response>
    /// <response code="401">The request does not contain a valid JWT access token.</response>
    /// <response code="403">The authenticated user does not have the Admin role.</response>
    /// <response code="404">No message source has the supplied database identifier.</response>
    /// <response code="500">An unexpected server or database error occurred.</response>
    [HttpDelete("delete/{messageSourceId:long}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(long messageSourceId,
        CancellationToken cancellationToken = default)
    {
        if (messageSourceId < 1)
        {
            return BadRequest("Message source ID must be at least 1.");
        }

        var messageSource = await _junkEmailCleanerContext.MessageSources
            .SingleOrDefaultAsync(candidate => candidate.MessageSourceId == messageSourceId, cancellationToken);

        if (messageSource is null)
        {
            return NotFound();
        }

        _junkEmailCleanerContext.MessageSources.Remove(messageSource);
        await _junkEmailCleanerContext.SaveChangesAsync(cancellationToken);

        return Ok(true);
    }

    private BadRequestObjectResult? ValidateBlockedSenderName(string? blockedSenderName)
    {
        if (string.IsNullOrWhiteSpace(blockedSenderName))
        {
            return BadRequest("Blocked sender name is required.");
        }

        return null;
    }

    private Task<bool> BlockedSenderNameExists(string blockedSenderName, long? excludedMessageSourceId,
        CancellationToken cancellationToken)
    {
        var query = _junkEmailCleanerContext.MessageSources
            .Where(messageSource => messageSource.BlockedSenderName == blockedSenderName);

        if (excludedMessageSourceId.HasValue)
        {
            query = query.Where(messageSource => messageSource.MessageSourceId != excludedMessageSourceId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    private static MessageSourceDetail CreateDetail(StoredMessageSource messageSource)
    {
        return new MessageSourceDetail
        {
            MessageSourceId = messageSource.MessageSourceId,
            BlockedSenderName = messageSource.BlockedSenderName,
            MessageSource = messageSource.MessageSource,
        };
    }
}
