namespace Otlob.Middlewares;

public static class CustomMiddlewares
{
    public static IApplicationBuilder UseRequestTimeMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RequestTimeMiddleware>();

        return builder;
    }

    public static IApplicationBuilder UseRateLimittingMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RateLimittingMiddleware>();

        return builder;
    }
}
