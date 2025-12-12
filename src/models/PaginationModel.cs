namespace Miccore.Pagination;

/// <summary>
/// Represents a paginated response containing items and metadata.
/// </summary>
/// <typeparam name="TModel">The type of items in the paginated collection.</typeparam>
public class PaginationModel<TModel>
{
    private const int MaxPageSize = 100;
    private int _pageSize;

    /// <summary>
    /// The number of items per page (maximum 100).
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// The current page number (1-indexed).
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The items for the current page.
    /// </summary>
    public List<TModel> Items { get; set; } = [];

    /// <summary>
    /// The URL to the previous page, or null if on the first page.
    /// </summary>
    public string? Prev { get; set; }

    /// <summary>
    /// The URL to the next page, or null if on the last page.
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// The property name used for sorting, or null if no sorting was applied.
    /// </summary>
    public string? SortedBy { get; set; }

    /// <summary>
    /// The sort direction ("asc" or "desc"), or null if no sorting was applied.
    /// </summary>
    public string? SortDirection { get; set; }
}