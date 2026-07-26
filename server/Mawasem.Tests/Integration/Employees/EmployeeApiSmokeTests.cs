using Mawasem.API.Authentication;
using Mawasem.Application.Features.Authentication.Contracts.Requests;
using Mawasem.Application.Features.Employees.Contracts.Requests;
using Mawasem.Application.Features.Employees.Contracts.Responses;
using Mawasem.Application.Features.Employees.Models;
using Mawasem.Domain.Identity;
using Mawasem.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Mawasem.Tests.Integration.Employees;

public sealed class EmployeeApiSmokeTests
    : IClassFixture<MawasemApiFactory>
{
    private readonly HttpClient _client;

    private string? _accessTokenCookie;

    public EmployeeApiSmokeTests(
        MawasemApiFactory factory )
    {
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
    public async Task AccessOptions_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var response =
            await _client.GetAsync(
                "/api/admin/employees/access-options");

        Assert.Equal(
            HttpStatusCode.Unauthorized ,
            response.StatusCode);
    }

    [Fact]
    public async Task DashboardMe_WithLoginCookie_ReturnsOk()
    {
        await LoginAsSuperAdminAsync();

        using var response =
            await SendAuthenticatedAsync(
                HttpMethod.Get ,
                "/api/admin/auth/me");

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)response.StatusCode}. " +
            $"Body: {responseBody}");
    }

    [Fact]
    public async Task AccessOptions_AsSuperAdmin_ReturnsAssignableRolesAndPermissions()
    {
        await LoginAsSuperAdminAsync();

        using var response =
            await SendAuthenticatedAsync(
                HttpMethod.Get ,
                "/api/admin/employees/access-options");

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        var accessOptions =
            await response.Content
                .ReadFromJsonAsync<
                    EmployeeAccessOptionsResponse>();

        Assert.NotNull(accessOptions);

        Assert.Contains(
            SystemRoles.Admin ,
            accessOptions.RoleNames);

        Assert.Contains(
            SystemRoles.SalesEmployee ,
            accessOptions.RoleNames);

        Assert.Contains(
            SystemRoles.DeliveryEmployee ,
            accessOptions.RoleNames);

        Assert.Contains(
            SystemRoles.SupportEmployee ,
            accessOptions.RoleNames);

        Assert.Contains(
            SystemRoles.StoreEmployee ,
            accessOptions.RoleNames);

        Assert.DoesNotContain(
            SystemRoles.SuperAdmin ,
            accessOptions.RoleNames);

        Assert.DoesNotContain(
            SystemRoles.Customer ,
            accessOptions.RoleNames);

        Assert.DoesNotContain(
            "CatalogManager" ,
            accessOptions.RoleNames);

        Assert.Contains(
            SystemPermissions.Employees.View ,
            accessOptions.PermissionNames);

        Assert.Contains(
            SystemPermissions.Employees.Create ,
            accessOptions.PermissionNames);

        Assert.Contains(
            SystemPermissions.Products.Create ,
            accessOptions.PermissionNames);

        Assert.All(
            accessOptions.PermissionNames ,
            permissionName =>
                Assert.Contains(
                    permissionName ,
                    SystemPermissions.All));
    }

    [Fact]
    public async Task CreateEmployee_WithSuperAdminRole_ReturnsInvalidRole()
    {
        await LoginAsSuperAdminAsync();

        var request =
            new CreateEmployeeRequest
            {
                FullNameAr =
                    "مدير نظام جديد" ,

                FullNameEn =
                    "New Super Admin" ,

                Email =
                    "new.superadmin.integration@mawasem.test" ,

                TemporaryPassword =
                    "Temporary1!" ,

                ConfirmTemporaryPassword =
                    "Temporary1!" ,

                RoleNames =
                    new[]
                    {
                        SystemRoles.SuperAdmin
                    } ,

                PermissionNames =
                    Array.Empty<string>()
            };

        await AssertInvalidRoleAsync(
            request ,
            SystemRoles.SuperAdmin);
    }

    [Fact]
    public async Task CreateEmployee_WithCatalogManagerRole_ReturnsInvalidRole()
    {
        await LoginAsSuperAdminAsync();

        var request =
            new CreateEmployeeRequest
            {
                FullNameAr =
                    "سارة خالد حسن" ,

                FullNameEn =
                    "Sara Khaled Hassan" ,

                Email =
                    "sara.khaled.integration@mawasem.test" ,

                TemporaryPassword =
                    "Temp@123" ,

                ConfirmTemporaryPassword =
                    "Temp@123" ,

                RoleNames =
                    new[]
                    {
                        "CatalogManager"
                    } ,

                PermissionNames =
                    new[]
                    {
                        SystemPermissions.Products.Create,
                        SystemPermissions.Products.Edit,
                        SystemPermissions.Brands.View,
                        SystemPermissions.Categories.View,
                        SystemPermissions.Collections.View,
                        SystemPermissions.Seasons.View
                    }
            };

        await AssertInvalidRoleAsync(
            request ,
            "CatalogManager");
    }

    [Fact]
    public async Task CreateEmployee_WithValidRole_ReturnsCreatedAndCanBeRetrieved()
    {
        await LoginAsSuperAdminAsync();

        var employeeEmail =
            $"sales.employee.{Guid.NewGuid():N}" +
            "@mawasem.test";

        var request =
            new CreateEmployeeRequest
            {
                FullNameAr =
                    "موظف مبيعات للاختبارات" ,

                FullNameEn =
                    "Integration Sales Employee" ,

                Email =
                    employeeEmail ,

                TemporaryPassword =
                    "Temporary1!" ,

                ConfirmTemporaryPassword =
                    "Temporary1!" ,

                RoleNames =
                    new[]
                    {
                        SystemRoles.SalesEmployee
                    } ,

                PermissionNames =
                    new[]
                    {
                        SystemPermissions.Orders.View
                    }
            };

        using var createResponse =
            await SendAuthenticatedAsync(
                HttpMethod.Post ,
                "/api/admin/employees" ,
                JsonContent.Create(request));

        var createResponseBody =
            await createResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode ==
            HttpStatusCode.Created ,
            $"Expected 201 Created but received " +
            $"{(int)createResponse.StatusCode}. " +
            $"Body: {createResponseBody}");

        var createdEmployee =
            await createResponse.Content
                .ReadFromJsonAsync<EmployeeResponse>();

        Assert.NotNull(createdEmployee);

        Assert.True(
            createdEmployee.Id > 0);

        Assert.Equal(
            request.FullNameAr ,
            createdEmployee.FullNameAr);

        Assert.Equal(
            request.FullNameEn ,
            createdEmployee.FullNameEn);

        Assert.Equal(
            employeeEmail ,
            createdEmployee.Email);

        Assert.False(
            createdEmployee.IsBlocked);

        Assert.True(
            createdEmployee.MustChangePassword);

        Assert.Contains(
            SystemRoles.SalesEmployee ,
            createdEmployee.Roles);

        Assert.Contains(
            SystemPermissions.Orders.View ,
            createdEmployee.DirectPermissions);

        Assert.Contains(
            SystemPermissions.Dashboard.Access ,
            createdEmployee.EffectivePermissions);

        Assert.Contains(
            SystemPermissions.Orders.View ,
            createdEmployee.EffectivePermissions);

        Assert.NotNull(
            createResponse.Headers.Location);

        Assert.EndsWith(
            $"/api/admin/employees/{createdEmployee.Id}" ,
            createResponse.Headers.Location.ToString() ,
            StringComparison.OrdinalIgnoreCase);

        using var getResponse =
            await SendAuthenticatedAsync(
                HttpMethod.Get ,
                $"/api/admin/employees/{createdEmployee.Id}");

        var getResponseBody =
            await getResponse.Content
                .ReadAsStringAsync();

        Assert.True(
            getResponse.StatusCode ==
            HttpStatusCode.OK ,
            $"Expected 200 OK but received " +
            $"{(int)getResponse.StatusCode}. " +
            $"Body: {getResponseBody}");

        var retrievedEmployee =
            await getResponse.Content
                .ReadFromJsonAsync<EmployeeResponse>();

        Assert.NotNull(retrievedEmployee);

        Assert.Equal(
            createdEmployee.Id ,
            retrievedEmployee.Id);

        Assert.Equal(
            employeeEmail ,
            retrievedEmployee.Email);

        Assert.Contains(
            SystemRoles.SalesEmployee ,
            retrievedEmployee.Roles);

        Assert.Contains(
            SystemPermissions.Orders.View ,
            retrievedEmployee.DirectPermissions);
    }

    private async Task LoginAsSuperAdminAsync()
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

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode ,
            $"SuperAdmin login failed. " +
            $"Status: {(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        Assert.True(
            response.Headers.TryGetValues(
                "Set-Cookie" ,
                out var setCookieHeaders) ,
            $"The login response did not set cookies. " +
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

        _accessTokenCookie =
            accessTokenCookieHeader!
                .Split(
                    ';' ,
                    2 ,
                    StringSplitOptions.TrimEntries)[0];

        Assert.StartsWith(
            $"{AuthenticationCookieNames.AccessToken}=" ,
            _accessTokenCookie ,
            StringComparison.OrdinalIgnoreCase);
    }

    private async Task AssertInvalidRoleAsync(
        CreateEmployeeRequest request ,
        string expectedRoleName )
    {
        using var response =
            await SendAuthenticatedAsync(
                HttpMethod.Post ,
                "/api/admin/employees" ,
                JsonContent.Create(request));

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode ==
            HttpStatusCode.BadRequest ,
            $"Expected 400 Bad Request but received " +
            $"{(int)response.StatusCode}. " +
            $"Body: {responseBody}");

        using var document =
            JsonDocument.Parse(
                responseBody);

        var root =
            document.RootElement;

        Assert.Equal(
            StatusCodes.Status400BadRequest ,
            root
                .GetProperty("status")
                .GetInt32());

        Assert.Equal(
            "Employee management request failed." ,
            root
                .GetProperty("title")
                .GetString());

        Assert.Equal(
            EmployeeManagementErrorCodes.InvalidRole ,
            root
                .GetProperty("code")
                .GetString());

        Assert.Equal(
            $"'{expectedRoleName}' is not assignable." ,
            root
                .GetProperty("detail")
                .GetString());
    }

    private async Task<HttpResponseMessage>
        SendAuthenticatedAsync(
            HttpMethod method ,
            string requestUri ,
            HttpContent? content = null )
    {
        Assert.False(
            string.IsNullOrWhiteSpace(
                _accessTokenCookie));

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
                _accessTokenCookie);

        Assert.True(
            cookieWasAdded);

        return await _client.SendAsync(request);
    }
}