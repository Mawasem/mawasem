using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Contracts.Responses;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Reports;

public sealed partial class ReportService
{
    public async Task<
        ReportResult<EmployeeReportSummaryResponse>>
        GetEmployeeSummaryAsync(
            GetEmployeeReportRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( !TryCalculateSkipCount(
                request.PageNumber ,
                request.PageSize ,
                out var skipCount ,
                out var paginationError) )
        {
            return ReportResult<
                EmployeeReportSummaryResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    paginationError);
        }

        if ( request.EmployeeId.HasValue &&
            request.EmployeeId.Value <= 0 )
        {
            return ReportResult<
                EmployeeReportSummaryResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    "Employee ID must be greater than zero.");
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
                request.ToDateUtc.Value )
        {
            return ReportResult<
                EmployeeReportSummaryResponse>.Failure(
                    ReportErrorCodes.InvalidDateRange ,
                    "The start date cannot be later than the end date.");
        }

        var search =
            NormalizeSearch(request.Search);

        if ( search?.Length > MaximumSearchLength )
        {
            return ReportResult<
                EmployeeReportSummaryResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    $"Search text cannot exceed " +
                    $"{MaximumSearchLength} characters.");
        }

        var requestedRole =
            ResolveDashboardRole(request.Role);

        if ( !string.IsNullOrWhiteSpace(request.Role) &&
            requestedRole is null )
        {
            return ReportResult<
                EmployeeReportSummaryResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    "The requested role is not a dashboard role.");
        }

        var dashboardRoleNames =
            SystemRoles.DashboardRoles.ToArray();

        var dashboardMemberships =
            from userRole
                in _dbContext.UserRoles.AsNoTracking()
            join role
                in _dbContext.Roles.AsNoTracking()
                on userRole.RoleId equals role.Id
            where
                role.Name != null &&
                dashboardRoleNames.Contains(role.Name)
            select new
            {
                userRole.UserId ,
                RoleName =
                    role.Name
            };

        var dashboardUserIds =
            dashboardMemberships
                .Select(membership =>
                    membership.UserId);

        var employeeQuery =
            _dbContext.Users
                .AsNoTracking()
                .Where(employee =>
                    dashboardUserIds.Contains(employee.Id));

        if ( request.EmployeeId.HasValue )
        {
            employeeQuery =
                employeeQuery.Where(employee =>
                    employee.Id ==
                    request.EmployeeId.Value);
        }

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            employeeQuery =
                employeeQuery.Where(employee =>
                    employee.FullNameAr.Contains(search) ||
                    employee.FullNameEn.Contains(search) ||
                    ( employee.Email != null &&
                      employee.Email.Contains(search) ));
        }

        if ( requestedRole is not null )
        {
            employeeQuery =
                employeeQuery.Where(employee =>
                    dashboardMemberships.Any(membership =>
                        membership.UserId == employee.Id &&
                        membership.RoleName ==
                            requestedRole));
        }

        var totalCount =
            await employeeQuery.CountAsync(
                cancellationToken);

        var employees =
            await employeeQuery
                .OrderBy(employee =>
                    employee.FullNameEn)
                .ThenBy(employee =>
                    employee.Id)
                .Skip(skipCount)
                .Take(request.PageSize)
                .Select(employee =>
                    new
                    {
                        employee.Id ,
                        employee.FullNameAr ,
                        employee.FullNameEn ,
                        employee.Email ,
                        employee.IsBlocked
                    })
                .ToArrayAsync(
                    cancellationToken);

        var employeeIds =
            employees
                .Select(employee =>
                    employee.Id)
                .ToArray();

        var roleRows =
            await (
                from userRole
                    in _dbContext.UserRoles.AsNoTracking()
                join role
                    in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id
                where
                    employeeIds.Contains(userRole.UserId) &&
                    role.Name != null &&
                    dashboardRoleNames.Contains(role.Name)
                select new
                {
                    userRole.UserId ,
                    RoleName =
                        role.Name
                })
                .ToArrayAsync(
                    cancellationToken);

        var orderActionQuery =
            _dbContext.OrderStatusHistories
                .AsNoTracking()
                .Where(history =>
                    history.ActorType ==
                        OrderStatusChangeActorType.DashboardUser &&
                    history.ChangedByUserId.HasValue &&
                    employeeIds.Contains(
                        history.ChangedByUserId.Value) &&
                    !history.Order.IsDeleted);

        if ( request.ActionStatus.HasValue )
        {
            orderActionQuery =
                orderActionQuery.Where(history =>
                    history.NewStatus ==
                    request.ActionStatus.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            orderActionQuery =
                orderActionQuery.Where(history =>
                    history.ChangedAtUtc >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            orderActionQuery =
                orderActionQuery.Where(history =>
                    history.ChangedAtUtc <=
                    request.ToDateUtc.Value);
        }

        var actionCounts =
            await orderActionQuery
                .GroupBy(history =>
                    new
                    {
                        EmployeeId =
                            history.ChangedByUserId!.Value ,

                        ActionStatus =
                            history.NewStatus
                    })
                .Select(group =>
                    new
                    {
                        group.Key.EmployeeId ,
                        group.Key.ActionStatus ,
                        Count =
                            group.Count()
                    })
                .ToArrayAsync(
                    cancellationToken);

        var items =
            employees
                .Select(employee =>
                {
                    var employeeActionCounts =
                        actionCounts
                            .Where(action =>
                                action.EmployeeId ==
                                employee.Id)
                            .OrderBy(action =>
                                action.ActionStatus)
                            .Select(action =>
                                new EmployeeOrderActionCountResponse
                                {
                                    ActionStatus =
                                        action.ActionStatus ,

                                    Count =
                                        action.Count
                                })
                            .ToArray();

                    var roles =
                        roleRows
                            .Where(role =>
                                role.UserId ==
                                employee.Id)
                            .Select(role =>
                                role.RoleName!)
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase)
                            .OrderBy(roleName =>
                                roleName ,
                                StringComparer.OrdinalIgnoreCase)
                            .ToArray();

                    return new EmployeeReportSummaryItemResponse
                    {
                        EmployeeId =
                            employee.Id ,

                        FullNameAr =
                            employee.FullNameAr ,

                        FullNameEn =
                            employee.FullNameEn ,

                        Email =
                            employee.Email ,

                        IsBlocked =
                            employee.IsBlocked ,

                        Roles =
                            roles ,

                        TotalOrderActions =
                            employeeActionCounts.Sum(action =>
                                action.Count) ,

                        OrderActions =
                            employeeActionCounts
                    };
                })
                .ToArray();

        var response =
            new EmployeeReportSummaryResponse
            {
                Items =
                    items ,

                PageNumber =
                    request.PageNumber ,

                PageSize =
                    request.PageSize ,

                TotalCount =
                    totalCount ,

                TotalPages =
                    CalculateTotalPages(
                        totalCount ,
                        request.PageSize)
            };

        return ReportResult<
            EmployeeReportSummaryResponse>.Success(
                response);
    }
}
