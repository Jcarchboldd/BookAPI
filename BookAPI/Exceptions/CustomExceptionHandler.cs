using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace BookAPI.Exceptions;

public class CustomExceptionHandler(
    ILogger<CustomExceptionHandler> logger,
    ProblemDetailsFactory problemDetailsFactory) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred at {time}", DateTime.UtcNow);

        var (title, detail, statusCode) = exception switch
        {
            ValidationException => (
                "Validation Error",
                exception.Message,
                StatusCodes.Status400BadRequest
            ),
            BadRequestException => (
                "Bad Request",
                exception.Message,
                StatusCodes.Status400BadRequest
            ),
            NotFoundException => (
                "Not Found",
                exception.Message,
                StatusCodes.Status404NotFound
            ),
            InternalServerException => (
                "Internal Server Error",
                exception.Message,
                StatusCodes.Status500InternalServerError),
            _ => (
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.",
                StatusCodes.Status500InternalServerError
            )
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: context.Request.Path
        );

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["validationErrors"] = validationException.Errors;
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}