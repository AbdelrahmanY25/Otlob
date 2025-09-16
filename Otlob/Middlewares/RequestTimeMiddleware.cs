namespace Otlob.Middlewares;

public class RequestTimeMiddleware(RequestDelegate next, ILogger<RequestTimeMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestTimeMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var ip = context.Connection.RemoteIpAddress!.MapToIPv4();

        await _next(context);

        stopwatch.Stop();        

        _logger.LogInformation($"IP [{ip}] Request [{context.Request.Method}] Path [{context.Request.Path}] took {stopwatch.ElapsedMilliseconds} ms");
    }
}