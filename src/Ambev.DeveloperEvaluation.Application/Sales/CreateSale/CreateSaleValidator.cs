using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

internal class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required.");

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
