using System.Linq.Expressions;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Common.Pagination;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source, 
        int pageNumber = 1, 
        int pageSize = 10,
        string? orderBy = null,
        bool isDescending = false,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            source = isDescending
                ? source.OrderByDescending(orderBy)
                : source.OrderBy(orderBy);
        }

        var count = await source.ToAsyncEnumerable().CountAsync(cancellationToken);
        var items = await source.ToAsyncEnumerable()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PaginatedList<T>> ToPagedListAsync<T>(
        this IFindFluent<T, T> findFluent,
        int pageNumber, 
        int pageSize,
        string? orderBy = null,
        bool isDescending = false)
    {
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            var sortDefinition = isDescending
                ? Builders<T>.Sort.Descending(orderBy)
                : Builders<T>.Sort.Ascending(orderBy);

            findFluent = findFluent.Sort(sortDefinition);
        }

        var items = await findFluent.ToListAsync();
        var pagedFluent = await items.ToAsyncEnumerable().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(pagedFluent, items.Count, pageNumber, pageSize);
    }

    /// <summary>
    /// Orders a typed queryable collection by ascending.
    /// </summary>
    /// <typeparam name="T">The type of the queryable collection.</typeparam>
    /// <param name="source">The queryable collection.</param>
    /// <param name="propertyName">The property of the type.</param>
    /// <returns>The queryable collection ordered.</returns>
    private static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderBy(ToLambda<T>(propertyName));
    }

    /// <summary>
    /// Orders a typed queryable collection by descending.
    /// </summary>
    /// <typeparam name="T">The type of the queryable collection.</typeparam>
    /// <param name="source">The queryable collection.</param>
    /// <param name="propertyName">The property of the type.</param>
    /// <returns>The queryable collection ordered.</returns>
    private static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderByDescending(ToLambda<T>(propertyName));
    }

    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
    }
}
