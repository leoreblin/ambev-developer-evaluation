using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.AddItem;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

[ApiController]
[Route("carts")]
public class CartsController : BaseController
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Retrieves the customer's cart.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{customerId:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<Cart>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> GetCart(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _cartService.GetCartAsync(customerId, cancellationToken);
        if (cart is null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        return Ok(cart);
    }

    /// <summary>
    /// Adds a product to the customer's cart.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{customerId:guid}")]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToCart(
        Guid customerId,
        [FromBody] AddItemDto request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity == 0)
        {
            throw new ValidationException("At least 1 product must be added in the cart.");
        }

        var theCart = await _cartService.GetCartAsync(customerId, cancellationToken);
        if (theCart is null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        await _cartService.AddToCartAsync(
            customerId,
            request.ProductId,
            request.Quantity,
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Clears the customer's cart.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{customerId:guid}")]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearCart(Guid customerId, CancellationToken cancellationToken)
    {
        var theCart = await _cartService.GetCartAsync(customerId, cancellationToken);
        if (theCart is null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        await _cartService.ClearCartAsync(customerId);
        return NoContent();
    }

    /// <summary>
    /// Removes a product from the customer's cart.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="productId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{customerId:guid}/remove/{productId:guid}")]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveProduct(Guid customerId, Guid productId, CancellationToken cancellationToken)
    {
        var theCart = await _cartService.GetCartAsync(customerId, cancellationToken);
        if (theCart is null)
        {
            throw new KeyNotFoundException("Cart not found.");
        }

        await _cartService.RemoveProductAsync(customerId, productId);
        return NoContent();
    }
}
