using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;

    public JwtValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path;
        if (path.StartsWithSegments("/api/auth/register") || path.StartsWithSegments("/api/auth/login"))
        {
            await _next(context);
            return;
        }

        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Access is denied due to invalid credentials.");
            return;
        }

        await _next(context);
    }
}