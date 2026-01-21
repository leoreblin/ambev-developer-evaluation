using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(sale => sale.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(sale => sale.BranchId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(sale => sale.Items).NotEmpty().WithMessage("At least one item is required.")
            .Must(items => items.Count > 0)
            .WithMessage("At least one item is required.")
            .ForEach(itemRule =>
            {
                itemRule.Must(item => item.Quantity > 0)
                    .WithMessage("Quantity must be greater than 0.");

                itemRule.Must(item => item.UnitPrice > 0)
                    .WithMessage("Unit price must be greater than 0.");

                itemRule.Must(item => item.ProductId != Guid.Empty)
                    .WithMessage("Product ID is required.");
            });
    }
}
