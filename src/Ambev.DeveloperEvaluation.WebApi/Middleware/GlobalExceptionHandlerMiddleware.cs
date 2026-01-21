using System.Net;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Represents the global exception handler middleware.
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the specified <see cref="Exception"/> for the specified <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The HTTP Context.</param>
    /// <param name="ex">The exception.</param>
    /// <returns>The HTTP response that is modified based on the exception.</returns>
    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var problemDetails = CreateProblemDetails(ex);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        string response = JsonSerializer.Serialize(problemDetails, JsonSerializerOptions);

        await context.Response.WriteAsync(response);
    }

    private static ProblemDetails CreateProblemDetails(Exception exception)
        => exception switch
        {
            ValidationException validationException => CreateValidationProblemDetails(validationException),

            DomainException domainException => new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Unprocessable Entity",
                Detail = domainException.Message
            },

            UnauthorizedAccessException unauthorizedException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = unauthorizedException.Message
            },

            KeyNotFoundException notFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFoundException.Message
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = exception.Message
            }
        };

    private static ValidationProblemDetails CreateValidationProblemDetails(ValidationException exception)
    {
        var validationFailures = exception.Errors?.ToList() ?? [];

        if (validationFailures.Count == 0 && !string.IsNullOrWhiteSpace(exception.Message))
        {
            return new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["General"] = [exception.Message]
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred."
            };
        }

        var errors = validationFailures
            .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? "General" : error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray()
            );

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred."
        };
    }
}
