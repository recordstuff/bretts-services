namespace bretts_services.Models.ViewModels;

/// <summary>Defines paging, sorting, and filtering for application logs.</summary>
public record LogSearchParameters
{
    /// <summary>Gets or sets the one-based page number.</summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>Gets or sets the maximum number of entries in a page.</summary>
    [Range(1, 250)]
    public int PageSize { get; set; } = 25;

    /// <summary>Gets or sets optional text matched across the complete log entry.</summary>
    public string? SearchText { get; set; }

    /// <summary>Gets or sets an optional inclusive lower timestamp bound.</summary>
    public DateTime? From { get; set; }

    /// <summary>Gets or sets an optional inclusive upper timestamp bound.</summary>
    public DateTime? To { get; set; }

    /// <summary>Gets or sets an optional exact Serilog level.</summary>
    public string? Level { get; set; }

    /// <summary>Gets or sets whether results are ordered newest first.</summary>
    public bool NewestFirst { get; set; } = true;

    /// <summary>Gets or sets structured attribute conditions. Every condition must match.</summary>
    public List<LogAttributeFilter> AttributeFilters { get; set; } = new();
}
