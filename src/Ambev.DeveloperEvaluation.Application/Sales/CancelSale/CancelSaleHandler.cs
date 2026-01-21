using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing <see cref="CancelSaleCommand"/> requests.
/// </summary>
internal sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand, Unit>
{
    private readonly ISaleRepository _saleRespository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleHandler"/> class.
    /// </summary>
    /// <param name="saleRespository">The sales repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CancelSaleHandler(ISaleRepository saleRespository, IUnitOfWork unitOfWork)
    {
        _saleRespository = saleRespository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(
        CancelSaleCommand request,
        CancellationToken cancellationToken)
    {
        var sale = await _saleRespository.GetByIdAsync(request.SaleId, cancellationToken)
            ?? throw new KeyNotFoundException("Sale does not exist.");

        sale.Cancel();

        _saleRespository.Update(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Unit.Task;
    }
}
