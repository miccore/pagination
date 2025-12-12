using System.ComponentModel;

namespace Miccore.Pagination;

/// <summary>
/// Query parameters for pagination and sorting.
/// </summary>
public class PaginationQuery
{
    /// <summary>
    /// Indicates whether pagination should be applied.
    /// When false, all items are returned without pagination.
    /// </summary>
    [DefaultValue(false)]
    public bool Paginate { get; set; }

    /// <summary>
    /// The page number to retrieve (1-indexed).
    /// </summary>
    [DefaultValue(1)]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The maximum number of items per page.
    /// </summary>
    [DefaultValue(10)]
    public int Limit { get; set; } = 10;

    /// <summary>
    /// The property name to sort by (e.g., "CreatedAt", "Name").
    /// When null or empty, no sorting is applied.
    /// </summary>
    /// <remarks>
    /// For deterministic pagination results, ensure the sort property is unique
    /// or combine with a secondary sort on a unique property (e.g., Id).
    /// </remarks>
    public string? OrderBy { get; set; }

    /// <summary>
    /// The sort direction: "asc" for ascending or "desc" for descending.
    /// Defaults to "asc".
    /// </summary>
    [DefaultValue("asc")]
    public string OrderDirection { get; set; } = "asc";
}