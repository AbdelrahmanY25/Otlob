namespace Otlob.Middlewares;

public class RateLimittingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    private readonly static Dictionary<string, (int Count, DateTime LastRequest)> _clients = [];
    private const int _limit = 30;
    private const int _windowSeconds = 10;

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (_clients.ContainsKey(clientIp))
        {
            var (count, lastRequest) = _clients[clientIp];

            if ((DateTime.Now - lastRequest).TotalSeconds > _windowSeconds)
            {
                _clients[clientIp] = (1, DateTime.Now);
                await _next(context);
                return;
            }

            if (count >= _limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "templates", "To-Many-Requests-Error-Page.html")));
            }
            else
            {
                _clients[clientIp] = (count + 1, DateTime.Now);
            }
        }
        else
        {
            _clients[clientIp] = (1, DateTime.Now);
        }

        await _next(context);
    }
}

