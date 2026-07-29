using Mawasem.Tests.Integration.Infrastructure;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Mawasem.Tests.Integration.Security;

public sealed class ProductionReadinessApiTests
    : IClassFixture<MawasemApiFactory>
{
    private readonly MawasemApiFactory
        _factory;

    public ProductionReadinessApiTests(
        MawasemApiFactory factory )
    {
        _factory = factory;
    }

    [Fact]
    public async Task LiveHealth_ReturnsHealthy()
    {
        using var client =
            _factory.CreateClient();

        using var response =
            await client.GetAsync(
                "/health/live");

        Assert.Equal(
            HttpStatusCode.OK ,
            response.StatusCode);

        var body =
            await response.Content
                .ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(
                body);

        Assert.Equal(
            "Healthy" ,
            document.RootElement
                .GetProperty("status")
                .GetString());
    }

    [Fact]
    public async Task ReadyHealth_ReturnsHealthy()
    {
        using var client =
            _factory.CreateClient();

        using var response =
            await client.GetAsync(
                "/health/ready");

        Assert.Equal(
            HttpStatusCode.OK ,
            response.StatusCode);

        var body =
            await response.Content
                .ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(
                body);

        Assert.Equal(
            "Healthy" ,
            document.RootElement
                .GetProperty("status")
                .GetString());
    }

    [Fact]
    public async Task Response_IncludesSecurityHeaders()
    {
        using var client =
            _factory.CreateClient();

        using var response =
            await client.GetAsync(
                "/health/live");

        Assert.Equal(
            "nosniff" ,
            GetHeader(
                response ,
                "X-Content-Type-Options"));

        Assert.Equal(
            "DENY" ,
            GetHeader(
                response ,
                "X-Frame-Options"));

        Assert.Equal(
            "no-referrer" ,
            GetHeader(
                response ,
                "Referrer-Policy"));

        Assert.Equal(
            "camera=(), microphone=(), geolocation=()" ,
            GetHeader(
                response ,
                "Permissions-Policy"));

        Assert.Equal(
            "none" ,
            GetHeader(
                response ,
                "X-Permitted-Cross-Domain-Policies"));

        Assert.Equal(
            "same-origin" ,
            GetHeader(
                response ,
                "Cross-Origin-Opener-Policy"));

        Assert.Equal(
            "same-site" ,
            GetHeader(
                response ,
                "Cross-Origin-Resource-Policy"));

        Assert.Equal(
            "0" ,
            GetHeader(
                response ,
                "X-XSS-Protection"));
    }

    [Fact]
    public async Task UploadPath_AllowsCrossOriginImageLoading()
    {
        using var client =
            _factory.CreateClient();

        using var response =
            await client.GetAsync(
                "/uploads/products/missing-image.jpg");

        Assert.Equal(
            HttpStatusCode.NotFound ,
            response.StatusCode);

        Assert.Equal(
            "cross-origin" ,
            GetHeader(
                response ,
                "Cross-Origin-Resource-Policy"));
    }

    [Fact]
    public async Task CorsPreflight_AllowedFrontendOrigin_Succeeds()
    {
        using var client =
            _factory.CreateClient();

        using var request =
            new HttpRequestMessage(
                HttpMethod.Options ,
                "/api/auth/me");

        request.Headers.TryAddWithoutValidation(
            "Access-Control-Request-Method" ,
            "GET");

        using var response =
            await client.SendAsync(
                request);

        Assert.Equal(
            HttpStatusCode.NoContent ,
            response.StatusCode);

        Assert.Equal(
            MawasemApiFactory.FrontendOrigin ,
            GetHeader(
                response ,
                "Access-Control-Allow-Origin"));

        Assert.Equal(
            "true" ,
            GetHeader(
                response ,
                "Access-Control-Allow-Credentials"));
    }

    [Fact]
    public async Task UnsafeRequest_AllowedOriginWithAuthenticationCookie_ReachesEndpoint()
    {
        using var client =
            _factory.CreateClient();

        using var request =
            CreateUnsafeRequest();

        request.Headers.TryAddWithoutValidation(
            "Cookie" ,
            "mawasem_customer_refresh_token=test-refresh-token");

        using var response =
            await client.SendAsync(
                request);

        Assert.Equal(
            HttpStatusCode.NotFound ,
            response.StatusCode);
    }

    [Fact]
    public async Task UnsafeRequest_AuthenticationCookieWithoutOrigin_IsRejected()
    {
        using var client =
            _factory.CreateClient();

        client.DefaultRequestHeaders.Remove(
            "Origin");

        using var request =
            CreateUnsafeRequest();

        request.Headers.TryAddWithoutValidation(
            "Cookie" ,
            "mawasem_customer_refresh_token=test-refresh-token");

        using var response =
            await client.SendAsync(
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden ,
            response.StatusCode);

        Assert.Equal(
            "security.origin_required" ,
            await GetProblemCodeAsync(
                response));
    }

    [Fact]
    public async Task UnsafeRequest_DisallowedOrigin_IsRejected()
    {
        using var client =
            _factory.CreateClient();

        client.DefaultRequestHeaders.Remove(
            "Origin");

        using var request =
            CreateUnsafeRequest();

        request.Headers.TryAddWithoutValidation(
            "Origin" ,
            "https://malicious.example");

        using var response =
            await client.SendAsync(
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden ,
            response.StatusCode);

        Assert.Equal(
            "security.origin_not_allowed" ,
            await GetProblemCodeAsync(
                response));
    }

    private static HttpRequestMessage
        CreateUnsafeRequest()
    {
        return new HttpRequestMessage(
            HttpMethod.Post ,
            "/api/auth/origin-test")
        {
            Content =
                new StringContent(
                    "{}" ,
                    Encoding.UTF8 ,
                    "application/json")
        };
    }

    private static async Task<string?>
        GetProblemCodeAsync(
            HttpResponseMessage response )
    {
        var body =
            await response.Content
                .ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(
                body);

        return document.RootElement
            .GetProperty("code")
            .GetString();
    }

    private static string GetHeader(
        HttpResponseMessage response ,
        string headerName )
    {
        Assert.True(
            response.Headers.TryGetValues(
                headerName ,
                out var values) ,
            $"The response header '{headerName}' was not found.");

        return Assert.Single(
            values);
    }
}