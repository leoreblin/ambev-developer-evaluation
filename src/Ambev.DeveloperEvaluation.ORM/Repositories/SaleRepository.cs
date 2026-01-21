using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Branch)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Sale?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Branch)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PaginatedList<Sale>> GetCustomerSalesAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        string? saleNumber,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Branch)
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId);

        if (!string.IsNullOrWhiteSpace(saleNumber))
        {
            query = query.Where(s => s.Number.ToLower().Contains(saleNumber.ToLower()));
        }

        return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public void Update(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Update(sale);
    }

    /// <inheritdoc/>
    public void MarkAsModified(Sale sale)
    {
        _context.Entry(sale).State = EntityState.Modified;
        foreach (var item in sale.Items)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
