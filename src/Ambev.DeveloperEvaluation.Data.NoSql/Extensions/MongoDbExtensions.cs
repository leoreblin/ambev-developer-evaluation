using Ambev.DeveloperEvaluation.Data.NoSql.Configurations;
using Ambev.DeveloperEvaluation.Data.NoSql.Context;
using Ambev.DeveloperEvaluation.Data.NoSql.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.ConnectionString));

            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(5);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            clientSettings.SocketTimeout = TimeSpan.FromSeconds(5);

            clientSettings.RetryReads = true;
            clientSettings.RetryWrites = true;

            return new MongoClient(clientSettings);
        });            

        services.AddSingleton<MongoDbContext>();

        services.AddHostedService<MongoIndexCreator>();

        return services;
    }
}
