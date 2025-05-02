using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookAPI.Exceptions;

public class CustomExceptionHandler(
    ILogger<CustomExceptionHandler> logger,
    ProblemDetailsFactory problemDetailsFactory) : IExceptionHandler
{
    private static readonly Dictionary<Type, (int StatusCode, string Title, LogLevel LogLevel)> ExceptionMap =
        new()
        {
            { typeof(ValidationException), (StatusCodes.Status400BadRequest, "Validation Error", LogLevel.Warning) },
            { typeof(BadRequestException), (StatusCodes.Status400BadRequest, "Bad Request", LogLevel.Warning) },
            { typeof(NotFoundException),    (StatusCodes.Status404NotFound,  "Not Found", LogLevel.Information) },
            { typeof(InternalServerException), (StatusCodes.Status500InternalServerError, "Internal Server Error", LogLevel.Error) }
        };

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var errorId = Guid.NewGuid().ToString();

        var (statusCode, title, logLevel) = ExceptionMap.TryGetValue(exception.GetType(), out var mapped)
            ? mapped
            : (StatusCodes.Status500InternalServerError, "Internal Server Error", LogLevel.Error);

        // Use the exception message if safe, otherwise use a fallback
        var detail = exception is ValidationException or BadRequestException or NotFoundException
            ? exception.Message
            : "An unexpected error occurred. Please try again later.";

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: statusCode,
            title: title,
            type: null, // Let factory use RFC link if available
            detail: detail,
            instance: context.Request.Path
        );

        // Add extensions
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
        problemDetails.Extensions["errorId"] = errorId;

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["validationErrors"] = validationException.Errors;
        }

        // Log extra internal details if present (InternalServerException)
        if (exception is InternalServerException { Details: not null } ise)
        {
            logger.Log(logLevel, exception,
                "Internal server error occurred. Details: {Details}, ErrorId: {ErrorId}, TraceId: {TraceId}",
                ise.Details, errorId, context.TraceIdentifier);
        }
        else
        {
            logger.Log(logLevel, exception,
                "Unhandled exception occurred. ErrorId: {ErrorId}, TraceId: {TraceId}",
                errorId, context.TraceIdentifier);
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}