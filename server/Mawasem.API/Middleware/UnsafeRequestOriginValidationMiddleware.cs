using Mawasem.API.Authentication;
using Mawasem.API.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mawasem.API.Middleware;

public sealed class UnsafeRequestOriginValidationMiddleware
{
    private readonly RequestDelegate _next;

    private readonly HashSet<string> _allowedOrigins;

    public UnsafeRequestOriginValidationMiddleware(
        RequestDelegate next ,
        IOptions<FrontendOptions> frontendOptions )
    {
        _next = next;

        _allowedOrigins =
            frontendOptions.Value.AllowedOrigins
                .Where(origin =>
                    !string.IsNullOrWhiteSpace(origin))
                .Select(NormalizeOrigin)
                .Where(origin =>
                    origin is not null)
                .Select(origin =>
                    origin!)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(
        HttpContext context )
    {
        if ( IsSafeMethod(
                context.Request.Method) )
        {
            await _next(context);
            return;
        }

        if ( !context.Request.Headers.TryGetValue(
                "Origin" ,
                out var originValues) )
        {
            if ( HasAuthenticationCookie(
                    context.Request) )
            {
                await RejectAsync(
                    context ,
                    title:
                        "Request origin required." ,
                    detail:
                        "An origin header is required for cookie-authenticated requests that modify data." ,
                    code:
                        "security.origin_required");

                return;
            }

            // Non-browser clients using bearer authentication do not
            // automatically attach browser authentication cookies and
            // therefore are not vulnerable to traditional CSRF attacks.
            await _next(context);
            return;
        }

        var origin =
            NormalizeOrigin(
                originValues.ToString());

        var requestOrigin =
            $"{context.Request.Scheme}://" +
            $"{context.Request.Host.Value}";

        var isAllowed =
            origin is not null &&
            ( string.Equals(
                    origin ,
                    requestOrigin ,
                    StringComparison.OrdinalIgnoreCase) ||
                _allowedOrigins.Contains(origin) );

        if ( isAllowed )
        {
            await _next(context);
            return;
        }

        await RejectAsync(
            context ,
            title:
                "Request origin rejected." ,
            detail:
                "The request origin is not allowed to modify this resource." ,
            code:
                "security.origin_not_allowed");
    }

    private static bool HasAuthenticationCookie(
        HttpRequest request )
    {
        return
            request.Cookies.ContainsKey(
                AuthenticationCookieNames.AccessToken) ||
            request.Cookies.ContainsKey(
                AuthenticationCookieNames.CustomerRefreshToken) ||
            request.Cookies.ContainsKey(
                AuthenticationCookieNames.DashboardRefreshToken);
    }

    private static async Task RejectAsync(
        HttpContext context ,
        string title ,
        string detail ,
        string code )
    {
        context.Response.StatusCode =
            StatusCodes.Status403Forbidden;

        context.Response.ContentType =
            "application/problem+json";

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status403Forbidden ,

                Title =
                    title ,

                Detail =
                    detail
            };

        problemDetails.Extensions["code"] =
            code;

        problemDetails.Extensions["traceId"] =
            context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(
            problemDetails ,
            context.RequestAborted);
    }

    private static bool IsSafeMethod(
        string method )
    {
        return HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method) ||
            HttpMethods.IsOptions(method) ||
            HttpMethods.IsTrace(method);
    }

    private static string? NormalizeOrigin(
        string? origin )
    {
        if ( string.IsNullOrWhiteSpace(origin) ||
            !Uri.TryCreate(
                origin.Trim() ,
                UriKind.Absolute ,
                out var uri) )
        {
            return null;
        }

        if ( uri.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment) )
        {
            return null;
        }

        return uri.GetLeftPart(
            UriPartial.Authority);
    }
}