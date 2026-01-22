using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

internal sealed class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Unit>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var sale = await _saleRepository.GetByIdForUpdateAsync(request.SaleId, cancellationToken)
            ?? throw new KeyNotFoundException("Sale does not exist.");

        if (sale.IsCancelled)
        {
            throw new DomainException("Cannot modify cancelled sale.");
        }

        var distinctProductIds = request.Items.Select(item => item.ProductId).Distinct();
        var productsExist = await _productRepository.ProductsExistAsync(distinctProductIds, cancellationToken);
        if (!productsExist)
        {
            throw new ValidationException("Some products do not exist.");
        }

        var products = await _productRepository.GetByIdsAsync(distinctProductIds, cancellationToken);
        var productLookup = products.ToDictionary(p => p.Id, p => p.Title);

        var saleItems = request.Items
            .Select(item =>
            {
                var productName = productLookup[item.ProductId];
                return new SaleItemDraft(item.ProductId, productName, item.Quantity, item.UnitPrice);
            })
            .ToList();

        sale.ReplaceItems(saleItems);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Unit.Task;
    }
}
