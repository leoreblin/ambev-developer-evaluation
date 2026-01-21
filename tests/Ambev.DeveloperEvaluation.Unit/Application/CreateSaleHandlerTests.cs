using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public sealed class CreateSaleHandlerTests
{
    private readonly CreateSaleHandler _handler;
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(
            _saleRepository,
            _userRepository,
            _productRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var invalidCommand = new CreateSaleCommand
        {
            CustomerId = Guid.Empty,
            BranchId = Guid.Empty,
            Items = new List<CreateSaleItemDto>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var command = CreateValidCommand();
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        ex.Message.Should().Contain("There's no customer of ID");
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenUserIsNotCustomer()
    {
        // Arrange
        var command = CreateValidCommand();
        var adminUser = new User { Role = UserRole.Admin };
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(adminUser);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        ex.Message.Should().Be("The user is not a customer.");
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenProductsDontExist()
    {
        // Arrange
        var command = CreateValidCommand();
        var customer = new User { Role = UserRole.Customer };
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.ProductsExistAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        ex.Message.Should().Be("Some products do not exist.");
    }

    [Fact]
    public async Task Handle_ShouldCreateSale_WhenAllConditionsAreMet()
    {
        // Arrange
        var command = CreateValidCommand();
        var customer = new User { Role = UserRole.Customer, Id = command.CustomerId };
        _userRepository.GetByIdAsync(command.CustomerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.ProductsExistAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldAddItemsToSale_WhenProductsExist()
    {
        // Arrange
        var command = CreateValidCommand();
        var customer = new User { Role = UserRole.Customer };
        _userRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _productRepository.ProductsExistAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.Items.Count == command.Items.Count),
            Arg.Any<CancellationToken>());
    }

    private static CreateSaleCommand CreateValidCommand()
    {
        return new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items =
            [
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10.50m
                },
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1,
                    UnitPrice = 3.75m
                },
            ]
        };
    }
}
