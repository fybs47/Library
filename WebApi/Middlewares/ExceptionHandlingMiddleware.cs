using System.Text.Json;
using Application.Exсeptions;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка во время обработки запроса");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        switch (exception)
        {
            case BadRequestException badRequestException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = badRequestException.Message }));

            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = notFoundException.Message }));

            case ConflictException conflictException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = conflictException.Message }));

            case UnauthorizedException unauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = unauthorizedException.Message }));

            case ForbiddenException forbiddenException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = forbiddenException.Message }));

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Внутренняя ошибка сервера" }));
        }
    }
}
