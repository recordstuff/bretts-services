namespace bretts_services.Models;

public record PaginationResult<T>
{
    public int Page { get; set; }
    public int PageCount { get; set; }

    public long TotalItemCount { get; set; }
    public List<T> Items { get; set; } = new();
}
