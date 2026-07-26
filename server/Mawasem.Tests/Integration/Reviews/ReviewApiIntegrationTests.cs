using Mawasem.API.Authentication;
using Mawasem.Application.Features.Authentication.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Responses;
using Mawasem.Application.Features.Reviews.Models;
using Mawasem.Domain.Catalog;
using Mawasem.Domain.Common.ValueObjects;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Persistence.Contexts;
using Mawasem.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mawasem.Tests.Integration.Reviews;

public sealed class ReviewApiIntegrationTests
    : IClassFixture<MawasemApiFactory>
{
    private const string CustomerPassword =
        "Customer1!";

    private static int _phoneSequence =
        10000000;

    private readonly MawasemApiFactory _factory;

    private readonly HttpClient _client;

    public ReviewApiIntegrationTests(
        MawasemApiFactory factory )
    {
        _factory = factory;

        _client =
            factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress =
                        new Uri("https://localhost") ,

                    AllowAutoRedirect = false ,

                    HandleCookies = false
                });
    }

    [Fact]
    public async Task CreateReview_WithoutAuthentication_ReturnsUnauthorized()
    {
        var request =
            new CreateReviewRequest
            {
                Rating = 5 ,
                Comment = "Excellent product."
            };

        using var response =
            await _client.PostAsJsonAsync(
                "/api/products/1/reviews" ,
                request);

        Assert.Equal(
            HttpStatusCode.Unauthorized ,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateReview_CustomerCanCreateMultipleReviewsWithoutOrder()
    {
        var productId =
            await SeedPublishedProductAsync();

        var customerCookie =
            await RegisterCustomerAsync();

        using var firstResponse =
            await SendAuthenticatedAsync(
                customerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 5 ,
                        Comment =
                            "  Excellent first review.  "
                    }));

        var firstBody =
            await firstResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            firstResponse.StatusCode ==
            HttpStatusCode.Created ,
            $"Expected 201 Created but received " +
            $"{(int)firstResponse.StatusCode}. " +
            $"Body: {firstBody}");

        var firstReview =
            JsonSerializer.Deserialize<
                CustomerReviewResponse>(
                    firstBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(firstReview);

        Assert.Equal(
            "Excellent first review." ,
            firstReview.Comment);

        using var secondResponse =
            await SendAuthenticatedAsync(
                customerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 4 ,
                        Comment =
                            "A second review for the same product."
                    }));

        var secondBody =
            await secondResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            secondResponse.StatusCode ==
            HttpStatusCode.Created ,
            $"Expected 201 Created but received " +
            $"{(int)secondResponse.StatusCode}. " +
            $"Body: {secondBody}");

        var secondReview =
            JsonSerializer.Deserialize<
                CustomerReviewResponse>(
                    secondBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(secondReview);

        Assert.NotEqual(
            firstReview.Id ,
            secondReview.Id);

        using var listResponse =
            await _client.GetAsync(
                $"/api/products/{productId}/reviews");

        var listResponseBody =
            await listResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            listResponse.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)listResponse.StatusCode}. " +
            $"Body: {listResponseBody}");

        var list =
            JsonSerializer.Deserialize<
                PagedReviewResponse<PublicReviewResponse>>(
                    listResponseBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(list);

        Assert.Equal(
            2 ,
            list.TotalCount);

        Assert.Contains(
            list.Items ,
            review =>
                review.Id == firstReview.Id);

        Assert.Contains(
            list.Items ,
            review =>
                review.Id == secondReview.Id);

        using var summaryResponse =
            await _client.GetAsync(
                $"/api/products/{productId}/reviews/summary");

        var summary =
            await summaryResponse.Content
                .ReadFromJsonAsync<ReviewSummaryResponse>();

        Assert.Equal(
            HttpStatusCode.OK ,
            summaryResponse.StatusCode);

        Assert.NotNull(summary);

        Assert.Equal(
            2 ,
            summary.TotalCount);

        Assert.Equal(
            4.5m ,
            summary.AverageRating);

        Assert.Equal(
            1 ,
            summary.Distribution
                .Single(item =>
                    item.Rating == 4)
                .Count);

        Assert.Equal(
            1 ,
            summary.Distribution
                .Single(item =>
                    item.Rating == 5)
                .Count);
    }

    [Fact]
    public async Task CreateReview_InvalidRatingAndComment_ReturnBadRequest()
    {
        var productId =
            await SeedPublishedProductAsync();

        var customerCookie =
            await RegisterCustomerAsync();

        using var invalidRatingResponse =
            await SendAuthenticatedAsync(
                customerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 6 ,
                        Comment = "Valid comment."
                    }));

        await AssertProblemCodeAsync(
            invalidRatingResponse ,
            HttpStatusCode.BadRequest ,
            ReviewErrorCodes.InvalidRating);

        using var invalidCommentResponse =
            await SendAuthenticatedAsync(
                customerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 5 ,
                        Comment = " x "
                    }));

        await AssertProblemCodeAsync(
            invalidCommentResponse ,
            HttpStatusCode.BadRequest ,
            ReviewErrorCodes.InvalidComment);
    }

    [Fact]
    public async Task ReviewOwnership_OtherCustomerCannotUpdateOrDeleteReview()
    {
        var productId =
            await SeedPublishedProductAsync();

        var ownerCookie =
            await RegisterCustomerAsync();

        var otherCustomerCookie =
            await RegisterCustomerAsync();

        using var createResponse =
            await SendAuthenticatedAsync(
                ownerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 5 ,
                        Comment = "Owner review."
                    }));

        var createdReview =
            await createResponse.Content
                .ReadFromJsonAsync<CustomerReviewResponse>();

        Assert.Equal(
            HttpStatusCode.Created ,
            createResponse.StatusCode);

        Assert.NotNull(createdReview);

        using var updateResponse =
            await SendAuthenticatedAsync(
                otherCustomerCookie ,
                HttpMethod.Put ,
                $"/api/reviews/{createdReview.Id}" ,
                JsonContent.Create(
                    new UpdateReviewRequest
                    {
                        Rating = 1 ,
                        Comment =
                            "Unauthorized replacement."
                    }));

        await AssertProblemCodeAsync(
            updateResponse ,
            HttpStatusCode.Forbidden ,
            ReviewErrorCodes.ReviewAccessDenied);

        using var deleteResponse =
            await SendAuthenticatedAsync(
                otherCustomerCookie ,
                HttpMethod.Delete ,
                $"/api/reviews/{createdReview.Id}");

        await AssertProblemCodeAsync(
            deleteResponse ,
            HttpStatusCode.Forbidden ,
            ReviewErrorCodes.ReviewAccessDenied);

        using var summaryResponse =
            await _client.GetAsync(
                $"/api/products/{productId}/reviews/summary");

        var summary =
            await summaryResponse.Content
                .ReadFromJsonAsync<ReviewSummaryResponse>();

        Assert.Equal(
            HttpStatusCode.OK ,
            summaryResponse.StatusCode);

        Assert.NotNull(summary);

        Assert.Equal(
            1 ,
            summary.TotalCount);

        Assert.Equal(
            5m ,
            summary.AverageRating);
    }

    [Fact]
    public async Task AdminModeration_HideRestoreAndDelete_UpdatePublicSummary()
    {
        var productId =
            await SeedPublishedProductAsync();

        var customerCookie =
            await RegisterCustomerAsync();

        using var createResponse =
            await SendAuthenticatedAsync(
                customerCookie ,
                HttpMethod.Post ,
                $"/api/products/{productId}/reviews" ,
                JsonContent.Create(
                    new CreateReviewRequest
                    {
                        Rating = 3 ,
                        Comment =
                            "Review requiring moderation."
                    }));

        var createdReview =
            await createResponse.Content
                .ReadFromJsonAsync<CustomerReviewResponse>();

        Assert.Equal(
            HttpStatusCode.Created ,
            createResponse.StatusCode);

        Assert.NotNull(createdReview);

        var adminCookie =
            await LoginAsSuperAdminAsync();

        using var hideResponse =
            await SendAuthenticatedAsync(
                adminCookie ,
                HttpMethod.Post ,
                $"/api/admin/reviews/{createdReview.Id}/hide" ,
                JsonContent.Create(
                    new HideReviewRequest
                    {
                        ModerationReason =
                            "Spam content."
                    }));

        var hideResponseBody =
            await hideResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            hideResponse.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)hideResponse.StatusCode}. " +
            $"Body: {hideResponseBody}");

        var hiddenReview =
            JsonSerializer.Deserialize<
                AdminReviewResponse>(
                    hideResponseBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(hiddenReview);

        Assert.False(
            hiddenReview.IsVisible);

        Assert.Equal(
            "Spam content." ,
            hiddenReview.ModerationReason);

        Assert.NotNull(
            hiddenReview.ModeratedAtUtc);

        await AssertPublicSummaryAsync(
            productId ,
            expectedCount: 0 ,
            expectedAverageRating: 0m);

        using var restoreResponse =
            await SendAuthenticatedAsync(
                adminCookie ,
                HttpMethod.Post ,
                $"/api/admin/reviews/{createdReview.Id}/restore");

        var restoreResponseBody =
            await restoreResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            restoreResponse.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)restoreResponse.StatusCode}. " +
            $"Body: {restoreResponseBody}");

        var restoredReview =
            JsonSerializer.Deserialize<
                AdminReviewResponse>(
                    restoreResponseBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(restoredReview);

        Assert.True(
            restoredReview.IsVisible);

        Assert.Null(
            restoredReview.ModerationReason);

        await AssertPublicSummaryAsync(
            productId ,
            expectedCount: 1 ,
            expectedAverageRating: 3m);

        using var deleteResponse =
            await SendAuthenticatedAsync(
                adminCookie ,
                HttpMethod.Delete ,
                $"/api/admin/reviews/{createdReview.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent ,
            deleteResponse.StatusCode);

        await AssertPublicSummaryAsync(
            productId ,
            expectedCount: 0 ,
            expectedAverageRating: 0m);
    }

    private async Task<int>
        SeedPublishedProductAsync()
    {
        await using var scope =
            _factory.Services.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<MawasemDbContext>();

        var now =
            DateTimeOffset.UtcNow;

        var suffix =
            Guid.NewGuid().ToString("N");

        var brand =
            new Brand
            {
                Name =
                    new LocalizedText(
                        $"Review Brand {suffix}" ,
                        $"علامة المراجعات {suffix}") ,

                Description =
                    new LocalizedText(
                        "Review integration-test brand." ,
                        "علامة تجارية لاختبارات المراجعات.") ,

                IsActive = true ,
                CreatedOn = now ,
                CreatedBy = "integration-test" ,
                IsDeleted = false
            };

        var season =
            new Season
            {
                Name =
                    new LocalizedText(
                        $"Review Season {suffix}" ,
                        $"موسم المراجعات {suffix}") ,

                Description =
                    new LocalizedText(
                        "Review integration-test season." ,
                        "موسم لاختبارات المراجعات.") ,

                IsActive = true ,
                CreatedOn = now ,
                CreatedBy = "integration-test" ,
                IsDeleted = false
            };

        var product =
            new Product
            {
                Name =
                    new LocalizedText(
                        $"Review Product {suffix}" ,
                        $"منتج المراجعات {suffix}") ,

                Description =
                    new LocalizedText(
                        "Product created for review tests." ,
                        "منتج تم إنشاؤه لاختبارات المراجعات.") ,

                OriginalPrice = 100m ,
                CurrentPrice = 90m ,
                IsPublished = true ,
                IsFeatured = false ,
                Slug =
                    $"review-product-{suffix}" ,

                Brand = brand ,
                Season = season ,
                CreatedOn = now ,
                CreatedBy = "integration-test" ,
                IsDeleted = false
            };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        return product.Id;
    }

    private async Task<string>
        RegisterCustomerAsync()
    {
        var sequence =
            Interlocked.Increment(
                ref _phoneSequence);

        var phoneNumber =
            $"010{sequence:D8}";

        var request =
            new RegisterCustomerRequest
            {
                FullNameAr =
                    $"عميل مراجعات {sequence}" ,

                FullNameEn =
                    $"Review Customer {sequence}" ,

                PhoneNumber =
                    phoneNumber ,

                Gender =
                    Gender.Male ,

                Password =
                    CustomerPassword ,

                ConfirmPassword =
                    CustomerPassword
            };

        using var response =
            await _client.PostAsJsonAsync(
                "/api/auth/register" ,
                request);

        return await ExtractAccessTokenCookieAsync(
            response ,
            "Customer registration");
    }

    private async Task<string>
        LoginAsSuperAdminAsync()
    {
        var request =
            new LoginAdminRequest
            {
                Email =
                    MawasemApiFactory.SuperAdminEmail ,

                Password =
                    MawasemApiFactory.SuperAdminPassword
            };

        using var response =
            await _client.PostAsJsonAsync(
                "/api/admin/auth/login" ,
                request);

        return await ExtractAccessTokenCookieAsync(
            response ,
            "SuperAdmin login");
    }

    private static async Task<string>
        ExtractAccessTokenCookieAsync(
            HttpResponseMessage response ,
            string operationName )
    {
        var responseBody =
            await response.Content
                .ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode ,
            $"{operationName} failed. " +
            $"Status: {(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        Assert.True(
            response.Headers.TryGetValues(
                "Set-Cookie" ,
                out var setCookieHeaders) ,
            $"{operationName} did not set cookies. " +
            $"Body: {responseBody}");

        var accessTokenCookieHeader =
            setCookieHeaders!
                .FirstOrDefault(header =>
                    header.StartsWith(
                        $"{AuthenticationCookieNames.AccessToken}=" ,
                        StringComparison.OrdinalIgnoreCase));

        Assert.False(
            string.IsNullOrWhiteSpace(
                accessTokenCookieHeader));

        return accessTokenCookieHeader!
            .Split(
                ';' ,
                2 ,
                StringSplitOptions.TrimEntries)[0];
    }

    private async Task<HttpResponseMessage>
        SendAuthenticatedAsync(
            string accessTokenCookie ,
            HttpMethod method ,
            string requestUri ,
            HttpContent? content = null )
    {
        using var request =
            new HttpRequestMessage(
                method ,
                requestUri)
            {
                Content = content
            };

        var cookieWasAdded =
            request.Headers.TryAddWithoutValidation(
                "Cookie" ,
                accessTokenCookie);

        Assert.True(
            cookieWasAdded);

        return await _client.SendAsync(
            request);
    }

    private async Task
        AssertPublicSummaryAsync(
            int productId ,
            int expectedCount ,
            decimal expectedAverageRating )
    {
        using var response =
            await _client.GetAsync(
                $"/api/products/{productId}/reviews/summary");

        var responseBody =
            await response.Content
                .ReadAsStringAsync();

        Assert.True(
            response.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        var summary =
            JsonSerializer.Deserialize<
                ReviewSummaryResponse>(
                    responseBody ,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(summary);

        Assert.Equal(
            expectedCount ,
            summary.TotalCount);

        Assert.Equal(
            expectedAverageRating ,
            summary.AverageRating);
    }

    private static async Task
        AssertProblemCodeAsync(
            HttpResponseMessage response ,
            HttpStatusCode expectedStatusCode ,
            string expectedErrorCode )
    {
        var responseBody =
            await response.Content
                .ReadAsStringAsync();

        Assert.True(
            response.StatusCode ==
            expectedStatusCode ,
            $"Expected {(int)expectedStatusCode} but received " +
            $"{(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        using var document =
            JsonDocument.Parse(
                responseBody);

        Assert.Equal(
            expectedErrorCode ,
            document.RootElement
                .GetProperty("code")
                .GetString());
    }
}