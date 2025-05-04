using System.Diagnostics;

namespace BookAPI.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        var requestId = Guid.NewGuid().ToString();

        logger.LogInformation("[START] Request {Method} {Path} | TraceId: {TraceId} | RequestId: {RequestId}",
            request.Method, request.Path, context.TraceIdentifier, requestId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await next(context);
            stopwatch.Stop();

            if (stopwatch.Elapsed.TotalSeconds > 3)
            {
                logger.LogWarning("[PERFORMANCE] Request {Method} {Path} took {ElapsedMilliseconds}ms",
                    request.Method, request.Path, stopwatch.ElapsedMilliseconds);
            }

            logger.LogInformation("[END] Request {Method} {Path} completed with status {StatusCode} in {ElapsedMilliseconds}ms",
                request.Method, request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "[ERROR] Request {Method} {Path} failed after {ElapsedMilliseconds}ms",
                request.Method, request.Path, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}