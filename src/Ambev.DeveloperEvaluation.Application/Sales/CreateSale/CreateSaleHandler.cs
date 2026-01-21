using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing <see cref="CreateSaleCommand"/> requests.
/// </summary>
internal sealed class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandler"/> class.
    /// </summary>
    /// <param name="saleRepository">The sales repository.</param>
    /// <param name="userRepository">The users repository.</param>
    /// <param name="productRepository">The products repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)        
            throw new ValidationException(validationResult.Errors);        

        var customer = await _userRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new ValidationException($"There's no customer of ID {request.CustomerId}.");

        if (customer.Role != UserRole.Customer)
            throw new ValidationException("The user is not a customer.");

        var sale = new Sale(
            Guid.NewGuid(),
            GenerateSaleNumber(),
            DateTime.UtcNow,
            customer.Id,
            request.BranchId);

        var distinctProductIds = request.Items.Select(item => item.ProductId).Distinct();
        var productsExist = await _productRepository.ProductsExistAsync(distinctProductIds, cancellationToken);
        if (!productsExist)
            throw new ValidationException("Some products do not exist.");

        var saleItems = request.Items
            .Select(item => new SaleItemDraft(item.ProductId, item.Quantity, item.UnitPrice))
            .ToList();

        sale.ReplaceItems(saleItems);

        await _saleRepository.CreateAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateSaleResult(sale.Id);
    }

    private static string GenerateSaleNumber()
        => $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss0fff}";
}
