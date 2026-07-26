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
        ReportResult<EmployeeOrderActionsResponse>>
        GetEmployeeOrderActionsAsync(
            int employeeId ,
            GetEmployeeOrderActionsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( employeeId <= 0 )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.EmployeeNotFound ,
                    "The dashboard employee was not found.");
        }

        if ( !TryCalculateSkipCount(
                request.PageNumber ,
                request.PageSize ,
                out var skipCount ,
                out var paginationError) )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    paginationError);
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
                request.ToDateUtc.Value )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.InvalidDateRange ,
                    "The start date cannot be later than the end date.");
        }

        var search =
            NormalizeSearch(request.Search);

        if ( search?.Length > MaximumSearchLength )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    $"Search text cannot exceed " +
                    $"{MaximumSearchLength} characters.");
        }

        var employee =
            await _dbContext.Users
                .AsNoTracking()
                .Where(candidate =>
                    candidate.Id == employeeId)
                .Select(candidate =>
                    new
                    {
                        candidate.Id ,
                        candidate.FullNameAr ,
                        candidate.FullNameEn ,
                        candidate.Email
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( employee is null )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.EmployeeNotFound ,
                    "The dashboard employee was not found.");
        }

        var dashboardRoleNames =
            SystemRoles.DashboardRoles.ToArray();

        var employeeRoles =
            await (
                from userRole
                    in _dbContext.UserRoles.AsNoTracking()
                join role
                    in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id
                where
                    userRole.UserId == employeeId &&
                    role.Name != null &&
                    dashboardRoleNames.Contains(role.Name)
                select role.Name)
                .Distinct()
                .OrderBy(roleName =>
                    roleName)
                .ToArrayAsync(
                    cancellationToken);

        if ( employeeRoles.Length == 0 )
        {
            return ReportResult<
                EmployeeOrderActionsResponse>.Failure(
                    ReportErrorCodes.EmployeeNotFound ,
                    "The dashboard employee was not found.");
        }

        var actionQuery =
            _dbContext.OrderStatusHistories
                .AsNoTracking()
                .Where(history =>
                    history.ActorType ==
                        OrderStatusChangeActorType.DashboardUser &&
                    history.ChangedByUserId ==
                        employeeId &&
                    !history.Order.IsDeleted);

        if ( request.ActionStatus.HasValue )
        {
            actionQuery =
                actionQuery.Where(history =>
                    history.NewStatus ==
                    request.ActionStatus.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            actionQuery =
                actionQuery.Where(history =>
                    history.ChangedAtUtc >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            actionQuery =
                actionQuery.Where(history =>
                    history.ChangedAtUtc <=
                    request.ToDateUtc.Value);
        }

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            actionQuery =
                actionQuery.Where(history =>
                    history.Order.OrderNumber.Contains(search) ||
                    history.Order.CustomerNameAr.Contains(search) ||
                    history.Order.CustomerNameEn.Contains(search) ||
                    ( history.Reason != null &&
                      history.Reason.Contains(search) ));
        }

        var totalCount =
            await actionQuery.CountAsync(
                cancellationToken);

        var items =
            await actionQuery
                .OrderByDescending(history =>
                    history.ChangedAtUtc)
                .ThenByDescending(history =>
                    history.Id)
                .Skip(skipCount)
                .Take(request.PageSize)
                .Select(history =>
                    new EmployeeOrderActionResponse
                    {
                        HistoryId =
                            history.Id ,

                        OrderId =
                            history.OrderId ,

                        OrderNumber =
                            history.Order.OrderNumber ,

                        PreviousStatus =
                            history.PreviousStatus ,

                        NewStatus =
                            history.NewStatus ,

                        ChangedAtUtc =
                            history.ChangedAtUtc ,

                        Reason =
                            history.Reason ,

                        TotalAmount =
                            history.Order.TotalAmount ,

                        CustomerNameAr =
                            history.Order.CustomerNameAr ,

                        CustomerNameEn =
                            history.Order.CustomerNameEn
                    })
                .ToArrayAsync(
                    cancellationToken);

        var response =
            new EmployeeOrderActionsResponse
            {
                EmployeeId =
                    employee.Id ,

                FullNameAr =
                    employee.FullNameAr ,

                FullNameEn =
                    employee.FullNameEn ,

                Email =
                    employee.Email ,

                Roles =
                    employeeRoles
                        .Where(roleName =>
                            roleName is not null)
                        .Select(roleName =>
                            roleName!)
                        .ToArray() ,

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
            EmployeeOrderActionsResponse>.Success(
                response);
    }
}
