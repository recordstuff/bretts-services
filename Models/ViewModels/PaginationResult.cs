namespace bretts_services.Models.ViewModels;

/// <summary>
/// Represents one page of results and the metadata needed to navigate the full result set.
/// </summary>
/// <typeparam name="T">The type of item contained in the page.</typeparam>
public record PaginationResult<T>
{
    /// <summary>
    /// Gets or sets the one-based page number represented by this result.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages available at the requested page size.
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of matching items across all pages.
    /// </summary>
    public long ItemCount { get; set; }

    /// <summary>
    /// Gets or sets the items in the current page.
    /// </summary>
    public List<T> Items { get; set; } = new();
}
