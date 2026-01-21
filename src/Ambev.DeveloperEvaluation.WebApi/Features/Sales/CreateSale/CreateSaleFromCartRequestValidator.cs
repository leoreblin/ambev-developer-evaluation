using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleFromCartRequestValidator : AbstractValidator<CreateSaleFromCartRequest>
{
    public CreateSaleFromCartRequestValidator()
    {
        RuleFor(sale => sale.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(sale => sale.BranchId).NotEmpty().WithMessage("Customer ID is required.");
    }
}
