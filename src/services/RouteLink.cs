namespace Miccore.Pagination;

/// <summary>
/// Extension methods for adding navigation links to pagination models.
/// </summary>
public static class RouteLink
{
    /// <summary>
    /// Adds Previous and Next navigation URLs to a pagination model.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="paginationModel">The pagination model to add links to.</param>
    /// <param name="controllerUrl">The base URL for the controller action (e.g., Url.RouteUrl(nameof(GetAllItems))).</param>
    /// <param name="query">The pagination query parameters.</param>
    /// <returns>The pagination model with navigation links populated.</returns>
    /// <example>
    /// <code>
    /// var result = await _context.Products.PaginateAsync(query);
    /// result.AddRouteLink(Url.RouteUrl(nameof(GetAllProducts)), query);
    /// </code>
    /// </example>
    public static PaginationModel<TModel> AddRouteLink<TModel>(
        this PaginationModel<TModel> paginationModel,
        string controllerUrl,
        PaginationQuery query) where TModel : class
    {
        if (!query.Paginate)
            return paginationModel;

        // Build base query parameters
        var baseParams = $"paginate={query.Paginate}&limit={query.Limit}";

        // Include sorting parameters if specified
        if (!string.IsNullOrEmpty(query.OrderBy))
        {
            baseParams += $"&orderBy={Uri.EscapeDataString(query.OrderBy)}&orderDirection={query.OrderDirection}";
        }

        // Add previous link if not on first page
        if (paginationModel.CurrentPage > 1)
        {
            paginationModel.Prev = $"{controllerUrl}?{baseParams}&page={query.Page - 1}";
        }

        // Add next link if not on last page
        if (paginationModel.CurrentPage < paginationModel.TotalPages)
        {
            paginationModel.Next = $"{controllerUrl}?{baseParams}&page={query.Page + 1}";
        }

        return paginationModel;
    }
}