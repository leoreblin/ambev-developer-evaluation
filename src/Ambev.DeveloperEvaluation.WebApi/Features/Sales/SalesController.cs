using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("sales")]
[Authorize]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ISaleRepository _saleRepository;
    private readonly ICartService _cartService;

    public SalesController(
        IMediator mediator,
        IMapper mapper,
        ISaleRepository saleRepository,
        ICartService cartService)
    {
        _mediator = mediator;
        _mapper = mapper;
        _saleRepository = saleRepository;
        _cartService = cartService;
    }

    /// <summary>
    /// Retrieves a sale by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid sale identifier.");
        }

        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        if (sale is null)
        {
            return NotFound("Sale not found.");
        }

        return Ok((GetSaleResponse)sale);
    }

    /// <summary>
    /// Retrieves the sales of a customer.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="saleNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("customers/{customerId:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCustomerSales(
        Guid customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? saleNumber = null,
        CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
        {
            return BadRequest("Invalid customer identifier.");
        }

        var sales = await _saleRepository.GetCustomerSalesAsync(
            customerId,
            pageNumber,
            pageSize,
            saleNumber,
            cancellationToken);

        var salesResponse = sales.Map(s => (GetSaleResponse)s);

        return OkPaginated(salesResponse);
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">The sale creation request.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The created sale details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var requestValidator = new CreateSaleRequestValidator();
        var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<Guid>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response.Id
        });
    }

    /// <summary>
    /// Creates a new sale from the customer's cart.
    /// </summary>
    /// <param name="request">The sale creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created sale details.</returns>
    [HttpPost("from-cart")]
    [ProducesResponseType(typeof(ApiResponseWithData<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateFromCart(
        [FromBody] CreateSaleFromCartRequest request, 
        CancellationToken cancellationToken)
    {
        var requestValidator = new CreateSaleFromCartRequestValidator();
        var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var cart = await _cartService.GetCartAsync(request.CustomerId, cancellationToken);
        if (cart is null)
        {
            return NotFound("There is no cart for the user.");
        }

        var command = new CreateSaleCommand
        {
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            Items = [.. cart.Items.Select(i => new CreateSaleItemDto
            {
                ProductId = i.Product.Id,
                UnitPrice = i.Product.Price,
                Quantity = i.Quantity
            })]
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<Guid>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response.Id
        });
    }

    /// <summary>
    /// Cancels a sale.
    /// </summary>
    /// <param name="id">The sale identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// 1) <see cref="NoContentResult"/> if it's cancelled. <br></br>
    /// 2) <see cref="NotFoundResult"/> if the sale was not found. <br></br>
    /// 3) <see cref="BadRequestResult"/> if the identifier was invalid.</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid sale identifier.");
        }

        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        if (sale is null)
        {
            return NotFound("Sale not found.");
        }

        if (sale.IsCancelled)
        {
            return BadRequest("The sale has already been cancelled.");
        }

        var command = new CancelSaleCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CancelSaleItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty || itemId == Guid.Empty)
        {
            return BadRequest("Invalid sale or sale item identifier.");
        }

        var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
        if (sale is null)
        {
            return NotFound("Sale not found.");
        }

        if (sale.IsCancelled)
        {
            return BadRequest("Cannot cancel an item of a cancelled sale.");
        }

        var command = new CancelSaleItemCommand(id, itemId);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates a sale.
    /// </summary>
    /// <param name="id">The sale identifier.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid sale identifier.");
        }

        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var command = new UpdateSaleCommand(id, request.Items);
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
