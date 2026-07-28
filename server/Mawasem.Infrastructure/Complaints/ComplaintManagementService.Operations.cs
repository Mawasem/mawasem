using Mawasem.Application.Features.Complaints.Contracts.Requests;
using Mawasem.Application.Features.Complaints.Contracts.Responses;
using Mawasem.Application.Features.Complaints.Models;
using Mawasem.Domain.Complaints;
using Mawasem.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.Complaints;

public sealed partial class ComplaintManagementService
{
    public async Task<ComplaintManagementResult<ComplaintListResponse>>
        GetListAsync(
            GetComplaintsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( request.PageNumber <= 0 )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "Page number must be greater than zero.");
        }

        if ( request.PageSize <= 0 ||
             request.PageSize > MaximumPageSize )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    $"Page size must be between 1 and {MaximumPageSize}.");
        }

        if ( request.CreatedByEmployeeId.HasValue &&
             request.CreatedByEmployeeId.Value <= 0 )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The employee filter is invalid.");
        }

        if ( request.FromDateUtc.HasValue &&
             request.ToDateUtc.HasValue &&
             request.FromDateUtc.Value >
             request.ToDateUtc.Value )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The start date cannot be later than the end date.");
        }

        var skipCount =
            (long)( request.PageNumber - 1 ) *
            request.PageSize;

        if ( skipCount > int.MaxValue )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The requested page is outside the supported range.");
        }

        var search =
            request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return ComplaintManagementResult<ComplaintListResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    $"Search text cannot exceed {MaximumSearchLength} characters.");
        }

        var complaintQuery =
            _dbContext.Complaints
                .AsNoTracking();

        if ( request.CreatedByEmployeeId.HasValue )
        {
            complaintQuery =
                complaintQuery.Where(complaint =>
                    complaint.CreatedByEmployeeId ==
                    request.CreatedByEmployeeId.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            complaintQuery =
                complaintQuery.Where(complaint =>
                    complaint.CreatedOn >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            complaintQuery =
                complaintQuery.Where(complaint =>
                    complaint.CreatedOn <=
                    request.ToDateUtc.Value);
        }

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            complaintQuery =
                complaintQuery.Where(complaint =>
                    complaint.CustomerName.Contains(search) ||
                    complaint.CustomerPhone.Contains(search) ||
                    complaint.ComplaintText.Contains(search) ||
                    complaint.CreatedByEmployee.FullNameAr.Contains(search) ||
                    complaint.CreatedByEmployee.FullNameEn.Contains(search));
        }

        var totalCount =
            await complaintQuery.CountAsync(
                cancellationToken);

        var items =
            await ProjectComplaints(complaintQuery)
                .OrderByDescending(complaint =>
                    complaint.CreatedOn)
                .ThenByDescending(complaint =>
                    complaint.Id)
                .Skip((int)skipCount)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

        var totalPages =
            totalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize);

        return ComplaintManagementResult<ComplaintListResponse>
            .Success(
                new ComplaintListResponse
                {
                    Items = items ,
                    PageNumber =
                        request.PageNumber ,
                    PageSize =
                        request.PageSize ,
                    TotalCount =
                        totalCount ,
                    TotalPages =
                        totalPages
                });
    }

    public async Task<ComplaintManagementResult<ComplaintResponse>>
        GetByIdAsync(
            int complaintId ,
            CancellationToken cancellationToken = default )
    {
        if ( complaintId <= 0 )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.NotFound ,
                    "The complaint was not found.");
        }

        var response =
            await ProjectComplaints(
                    _dbContext.Complaints
                        .AsNoTracking()
                        .Where(complaint =>
                            complaint.Id ==
                            complaintId))
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( response is null )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.NotFound ,
                    "The complaint was not found.");
        }

        return ComplaintManagementResult<ComplaintResponse>
            .Success(response);
    }

    public async Task<ComplaintManagementResult<ComplaintResponse>>
        CreateAsync(
            int actorUserId ,
            CreateComplaintRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( actorUserId <= 0 )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The authenticated dashboard account is invalid.");
        }

        var customerName =
            request.CustomerName?.Trim()
            ?? string.Empty;

        var customerPhone =
            request.CustomerPhone?.Trim()
            ?? string.Empty;

        var complaintText =
            request.ComplaintText?.Trim()
            ?? string.Empty;

        if ( string.IsNullOrWhiteSpace(customerName) )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The customer name is required.");
        }

        if ( customerName.Length >
             MaximumCustomerNameLength )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    $"The customer name cannot exceed {MaximumCustomerNameLength} characters.");
        }

        if ( string.IsNullOrWhiteSpace(customerPhone) )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The customer phone is required.");
        }

        if ( customerPhone.Length >
             MaximumCustomerPhoneLength )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    $"The customer phone cannot exceed {MaximumCustomerPhoneLength} characters.");
        }

        if ( string.IsNullOrWhiteSpace(complaintText) )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The complaint text is required.");
        }

        if ( complaintText.Length >
             MaximumComplaintTextLength )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    $"The complaint text cannot exceed {MaximumComplaintTextLength} characters.");
        }

        var employeeExists =
            await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.Id == actorUserId ,
                    cancellationToken);

        if ( !employeeExists )
        {
            return ComplaintManagementResult<ComplaintResponse>
                .Failure(
                    ComplaintManagementErrorCodes.InvalidRequest ,
                    "The authenticated dashboard account was not found.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var complaint =
            new Complaint
            {
                CustomerName =
                    customerName ,
                CustomerPhone =
                    customerPhone ,
                ComplaintText =
                    complaintText ,
                CreatedByEmployeeId =
                    actorUserId ,
                CreatedOn =
                    now ,
                CreatedBy =
                    actorUserId.ToString(
                        CultureInfo.InvariantCulture) ,
                IsDeleted =
                    false
            };

        _dbContext.Complaints.Add(complaint);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var response =
            await ProjectComplaints(
                    _dbContext.Complaints
                        .AsNoTracking()
                        .Where(existingComplaint =>
                            existingComplaint.Id ==
                            complaint.Id))
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The complaint was created but could not be reloaded.");
        }

        return ComplaintManagementResult<ComplaintResponse>
            .Success(response);
    }

    private static IQueryable<ComplaintResponse>
        ProjectComplaints(
            IQueryable<Complaint> query )
    {
        return query.Select(complaint =>
            new ComplaintResponse
            {
                Id =
                    complaint.Id ,
                CustomerName =
                    complaint.CustomerName ,
                CustomerPhone =
                    complaint.CustomerPhone ,
                ComplaintText =
                    complaint.ComplaintText ,
                CreatedByEmployeeId =
                    complaint.CreatedByEmployeeId ,
                CreatedByEmployeeNameAr =
                    complaint.CreatedByEmployee.FullNameAr ,
                CreatedByEmployeeNameEn =
                    complaint.CreatedByEmployee.FullNameEn ,
                CreatedOn =
                    complaint.CreatedOn
            });
    }
}
