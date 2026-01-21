using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

internal sealed class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, Unit>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        if (request.SaleId == Guid.Empty || request.SaleItemId == Guid.Empty)
        {
            throw new ValidationException("Invalid sale or sale item identifier.");
        }

        Sale sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken)
            ?? throw new KeyNotFoundException("Sale does not exist.");

        if (sale.IsCancelled)
            throw new DomainException("Cannot cancel an item of a cancelled sale.");

        if (sale.Items.All(item => item.Id != request.SaleItemId))
        {
            throw new KeyNotFoundException("Sale item does not exist.");
        }

        sale.CancelItem(request.SaleItemId);

        _saleRepository.MarkAsModified(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await Unit.Task;
    }
}
