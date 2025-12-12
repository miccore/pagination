using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Miccore.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for IQueryable to provide pagination, sorting, and streaming capabilities.
/// </summary>
public static class PaginationExtension
{
    /// <summary>
    /// Applies dynamic sorting to a query based on property name and direction.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="query">The queryable to sort.</param>
    /// <param name="orderBy">The property name to sort by. If null or empty, the original query is returned.</param>
    /// <param name="orderDirection">The sort direction: "asc" or "desc". Defaults to "asc".</param>
    /// <returns>The sorted queryable, or the original query if no valid sort property is specified.</returns>
    /// <remarks>
    /// <para>
    /// <strong>Important:</strong> For deterministic pagination results, ensure the sort property produces unique values
    /// or add a secondary sort on a unique property (e.g., Id). Without unique sorting, items may be duplicated
    /// or skipped across pages.
    /// </para>
    /// <example>
    /// <code>
    /// // Recommended: Always include a unique secondary sort
    /// var query = context.Products
    ///     .ApplySort("Price", "asc")
    ///     .ThenBy(x => x.Id); // Ensures deterministic order
    /// </code>
    /// </example>
    /// </remarks>
    public static IQueryable<TModel> ApplySort<TModel>(
        this IQueryable<TModel> query,
        string? orderBy,
        string orderDirection = "asc") where TModel : class
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query;

        var property = typeof(TModel).GetProperty(
            orderBy,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            return query;

        var parameter = Expression.Parameter(typeof(TModel), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var methodName = orderDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending"
            : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(TModel), property.PropertyType],
            query.Expression,
            Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<TModel>(resultExpression);
    }

    /// <summary>
    /// Asynchronously paginates a query with optional sorting.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="query">The queryable to paginate.</param>
    /// <param name="paginationQuery">The pagination and sorting parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A pagination model containing the results and metadata.</returns>
    public static async Task<PaginationModel<TModel>> PaginateAsync<TModel>(
        this IQueryable<TModel> query,
        PaginationQuery paginationQuery,
        CancellationToken cancellationToken = default) where TModel : class
    {
        // Apply sorting if specified
        query = query.ApplySort(paginationQuery.OrderBy, paginationQuery.OrderDirection);

        var paged = new PaginationModel<TModel>();

        // Ensure page is at least 1
        paginationQuery.Page = paginationQuery.Page < 1 ? 1 : paginationQuery.Page;

        paged.CurrentPage = paginationQuery.Page;
        paged.PageSize = paginationQuery.Limit;
        paged.TotalItems = await query.CountAsync(cancellationToken);

        // Store sorting info
        paged.SortedBy = paginationQuery.OrderBy;
        paged.SortDirection = paginationQuery.OrderDirection;

        var startRow = (paginationQuery.Page - 1) * paginationQuery.Limit;

        paged.Items = paginationQuery.Paginate
            ? await query.Skip(startRow).Take(paginationQuery.Limit).ToListAsync(cancellationToken)
            : await query.ToListAsync(cancellationToken);

        paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)paginationQuery.Limit);

        return paged;
    }

    /// <summary>
    /// Synchronously paginates a query with optional sorting.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="query">The queryable to paginate.</param>
    /// <param name="paginationQuery">The pagination and sorting parameters.</param>
    /// <returns>A pagination model containing the results and metadata.</returns>
    /// <remarks>
    /// <strong>Warning:</strong> This method blocks the calling thread. 
    /// Prefer <see cref="PaginateAsync{TModel}"/> for async scenarios.
    /// </remarks>
    public static PaginationModel<TModel> Paginate<TModel>(
        this IQueryable<TModel> query,
        PaginationQuery paginationQuery) where TModel : class
    {
        // Apply sorting if specified
        query = query.ApplySort(paginationQuery.OrderBy, paginationQuery.OrderDirection);

        var paged = new PaginationModel<TModel>();

        // Ensure page is at least 1
        paginationQuery.Page = paginationQuery.Page < 1 ? 1 : paginationQuery.Page;

        paged.CurrentPage = paginationQuery.Page;
        paged.PageSize = paginationQuery.Limit;
        paged.TotalItems = query.Count();

        // Store sorting info
        paged.SortedBy = paginationQuery.OrderBy;
        paged.SortDirection = paginationQuery.OrderDirection;

        var startRow = (paginationQuery.Page - 1) * paginationQuery.Limit;

        paged.Items = paginationQuery.Paginate
            ? query.Skip(startRow).Take(paginationQuery.Limit).ToList()
            : query.ToList();

        paged.TotalPages = (int)Math.Ceiling(paged.TotalItems / (double)paginationQuery.Limit);

        return paged;
    }

    /// <summary>
    /// Streams all items from a query as an async enumerable with optional sorting.
    /// Ideal for large datasets where you don't want to load all items into memory.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="query">The queryable to stream.</param>
    /// <param name="orderBy">Optional property name to sort by.</param>
    /// <param name="orderDirection">Sort direction: "asc" or "desc". Defaults to "asc".</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of items.</returns>
    public static async IAsyncEnumerable<TModel> AsStreamAsync<TModel>(
        this IQueryable<TModel> query,
        string? orderBy = null,
        string orderDirection = "asc",
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where TModel : class
    {
        query = query.ApplySort(orderBy, orderDirection);

        await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Streams paginated items from a query as an async enumerable.
    /// Returns only items for the specified page without loading all data.
    /// </summary>
    /// <typeparam name="TModel">The entity type.</typeparam>
    /// <param name="query">The queryable to paginate and stream.</param>
    /// <param name="paginationQuery">The pagination and sorting parameters.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of items for the requested page.</returns>
    public static async IAsyncEnumerable<TModel> PaginateAsStreamAsync<TModel>(
        this IQueryable<TModel> query,
        PaginationQuery paginationQuery,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where TModel : class
    {
        query = query.ApplySort(paginationQuery.OrderBy, paginationQuery.OrderDirection);

        if (!paginationQuery.Paginate)
        {
            await foreach (var item in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
            yield break;
        }

        var skip = (paginationQuery.Page - 1) * paginationQuery.Limit;

        await foreach (var item in query
            .Skip(skip)
            .Take(paginationQuery.Limit)
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
}