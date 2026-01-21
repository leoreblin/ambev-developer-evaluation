using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public sealed class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.")
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
