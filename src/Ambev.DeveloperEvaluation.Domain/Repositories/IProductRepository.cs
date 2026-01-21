using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Interface for the product repository.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets paginated products.
    /// </summary>
    /// <param name="pageNumber">The number of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="orderBy">The property to sort by.</param>
    /// <param name="isDescending">The sort direction.</param>
    /// <returns></returns>
    Task<PaginatedList<Product>> GetPaginatedAsync(
        int pageNumber, 
        int pageSize,
        string? orderBy = null,
        bool isDescending = false,
        string? term = null);

    /// <summary>
    /// Checks if products exist.
    /// </summary>
    /// <param name="productIds">The products identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<bool> ProductsExistAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Product?> GetByIdAsync(
        Guid productId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a collection of products by IDs.
    /// </summary>
    /// <param name="productIds">The list of products identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<Product>> GetByIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);
}
