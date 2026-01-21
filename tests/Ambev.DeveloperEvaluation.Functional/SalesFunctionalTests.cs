using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional;

public class SalesFunctionalTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public SalesFunctionalTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndUpdateSale_ShouldReturnExpectedStatuses()
    {
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        await SeedAsync(customerId, branchId);

        var client = _factory.CreateClient();
        var productId = Guid.NewGuid();

        var createRequest = new
        {
            customerId,
            branchId,
            items = new[]
            {
                new { productId, quantity = 4, unitPrice = 10m }
            }
        };

        var createResponse = await client.PostAsJsonAsync("/sales", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createPayload = await ReadResponseAsync<ApiResponseWithData<Guid>>(createResponse.Content);
        Assert.NotNull(createPayload);
        Assert.NotEqual(Guid.Empty, createPayload!.Data);

        var updateRequest = new
        {
            items = new[]
            {
                new { productId, quantity = 5, unitPrice = 10m }
            }
        };

        var updateResponse = await client.PutAsJsonAsync($"/sales/{createPayload.Data}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    private async Task SeedAsync(Guid customerId, Guid branchId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        context.Database.EnsureCreated();

        context.Users.Add(new User
        {
            Id = customerId,
            Username = "Customer Test",
            Email = "customer@test.com",
            Phone = "(11) 99999-9999",
            Password = "Pass@word1",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        });

        var branch = new Branch("Main Branch", "12345678901234") { Id = branchId };
        context.Branches.Add(branch);

        await context.SaveChangesAsync();
    }

    private static async Task<T?> ReadResponseAsync<T>(HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private sealed class ApiResponseWithData<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
