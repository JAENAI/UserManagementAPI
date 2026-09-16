namespace UserManagementAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestLoggingMiddleware> logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Incoming request: {Method} {Path}.", context.Request.Method, context.Request.Path);

        await next(context);

        logger.LogInformation(
            "Outgoing response: {Method} {Path} returned {StatusCode}.",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode);
    }
}