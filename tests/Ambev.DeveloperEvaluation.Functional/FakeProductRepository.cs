using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Functional;

public sealed class FakeProductRepository : IProductRepository
{
    public Task<PaginatedList<Product>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? orderBy = null,
        bool isDescending = false,
        string? term = null)
        => Task.FromResult(new PaginatedList<Product>([], 0, pageNumber, pageSize));

    public Task<bool> ProductsExistAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
        => Task.FromResult(true);

    public Task<Product?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
        => Task.FromResult<Product?>(null);

    public Task<IEnumerable<Product>> GetByIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var products = productIds
            .Distinct()
            .Select(id => new Product
            {
                Id = id,
                Title = $"Product {id}",
                Price = 10m
            })
            .ToList();

        return Task.FromResult<IEnumerable<Product>>(products);
    }
}
