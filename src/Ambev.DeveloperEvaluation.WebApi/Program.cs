using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Data.NoSql.Configurations;
using Ambev.DeveloperEvaluation.Data.NoSql.Context;
using Ambev.DeveloperEvaluation.Data.NoSql.Extensions;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            // PostgreSQL Configuration
            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(OrmLayer).Assembly.FullName)
                )
            );

            // MongoDB Configuration
            builder.Services.AddOptions<MongoDbSettings>()
               .BindConfiguration(MongoDbSettings.ConfigurationSection)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            builder.Services.AddMongoDb();

            // Redis Configuration
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    builder.Configuration.GetConnectionString("Redis")!
                )
            );

            // JWT Authentication Configuration
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            // AutoMapper and MediatR Configuration
            builder.Services.AddAutoMapper(
                typeof(Program).Assembly,
                typeof(ApplicationLayer).Assembly,
                typeof(OrmLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
            var app = builder.Build();

            // Apply Migrations
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            MigrationInitializer.ApplyMigrations(services);

            // Configure the HTTP request pipeline.
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await SeedDataAsync(app);
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task SeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetService<DefaultContext>();
        var mongoDbContext = services.GetService<MongoDbContext>();
        await DefaultContextSeed.SeedAsync(dbContext);
    }
}
