using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Application.Exсeptions;

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
            throw new UnauthorizedException("Unauthorized: Access is denied due to invalid credentials.");
        }

        await _next(context);
    }
}