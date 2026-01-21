using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository.
    /// </summary>
    /// <param name="sale">The sale.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its identifier.
    /// </summary>
    /// <param name="id">The sale identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its identifier with change tracking enabled.
    /// </summary>
    /// <param name="id">The sale identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Sale?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the sales of a customer.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="saleNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PaginatedList<Sale>> GetCustomerSalesAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        string? saleNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale"></param>
    /// <param name="cancellationToken"></param>
    void Update(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a sale as modified in the repository.
    /// </summary>
    /// <param name="sale"></param>
    void MarkAsModified(Sale sale);
}
