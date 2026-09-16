using System.Security.Cryptography;
using System.Text;

namespace UserManagementAPI.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate next;
    private readonly string expectedToken;
    private readonly ILogger<TokenAuthenticationMiddleware> logger;

    public TokenAuthenticationMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<TokenAuthenticationMiddleware> logger)
    {
        this.next = next;
        expectedToken = configuration["Authentication:Token"] ?? string.Empty;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        var token = authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorization["Bearer ".Length..].Trim()
            : string.Empty;

        if (string.IsNullOrEmpty(expectedToken) || !TokensMatch(token, expectedToken))
        {
            logger.LogWarning("Rejected unauthorized request: {Method} {Path}.", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized." });
            return;
        }

        await next(context);
    }

    private static bool TokensMatch(string token, string expectedToken)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var expectedTokenBytes = Encoding.UTF8.GetBytes(expectedToken);
        return tokenBytes.Length == expectedTokenBytes.Length
            && CryptographicOperations.FixedTimeEquals(tokenBytes, expectedTokenBytes);
    }
}