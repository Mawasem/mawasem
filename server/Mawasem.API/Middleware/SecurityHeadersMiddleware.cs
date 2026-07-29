namespace Mawasem.API.Middleware;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(
        RequestDelegate next )
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context )
    {
        context.Response.OnStarting(
            () =>
            {
                var headers =
                    context.Response.Headers;

                headers.TryAdd(
                    "X-Content-Type-Options" ,
                    "nosniff");

                headers.TryAdd(
                    "X-Frame-Options" ,
                    "DENY");

                headers.TryAdd(
                    "Referrer-Policy" ,
                    "no-referrer");

                headers.TryAdd(
                    "Permissions-Policy" ,
                    "camera=(), microphone=(), geolocation=()");

                headers.TryAdd(
                    "X-Permitted-Cross-Domain-Policies" ,
                    "none");

                headers.TryAdd(
                    "Cross-Origin-Opener-Policy" ,
                    "same-origin");

                headers.TryAdd(
                    "Cross-Origin-Resource-Policy" ,
                    context.Request.Path.StartsWithSegments(
                        "/uploads")
                        ? "cross-origin"
                        : "same-site");

                headers.TryAdd(
                    "X-XSS-Protection" ,
                    "0");

                return Task.CompletedTask;
            });

        await _next(context);
    }
}