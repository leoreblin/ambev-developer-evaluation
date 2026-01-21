using Ambev.DeveloperEvaluation.Data.NoSql.Context;
using Ambev.DeveloperEvaluation.Data.NoSql.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Services;

public class MongoIndexCreator : IHostedService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoIndexCreator> _logger;

    public MongoIndexCreator(MongoDbContext context, ILogger<MongoIndexCreator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await CreateProductIndexes(cancellationToken);
            _logger.LogInformation("MongoDB indexes created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MongoDB indexes.");
            throw;
        }
    }

    private async Task CreateProductIndexes(CancellationToken cancellationToken)
    {
        var productIndexes = new List<CreateIndexModel<ProductDocument>>
        {
            new(Builders<ProductDocument>.IndexKeys.Ascending(x => x.ExternalId)),
            new(Builders<ProductDocument>.IndexKeys.Text(x => x.Title))
        };

        await _context.Products.Indexes
            .CreateManyAsync(productIndexes, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
