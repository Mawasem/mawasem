using Mawasem.Application.Features.Complaints.Contracts.Requests;
using Mawasem.Application.Features.Complaints.Contracts.Responses;
using Mawasem.Application.Features.Complaints.Models;

namespace Mawasem.Application.Features.Complaints.Interfaces;

public interface IComplaintManagementService
{
    Task<ComplaintManagementResult<ComplaintListResponse>>
        GetListAsync(
            GetComplaintsRequest request ,
            CancellationToken cancellationToken = default );

    Task<ComplaintManagementResult<ComplaintResponse>>
        GetByIdAsync(
            int complaintId ,
            CancellationToken cancellationToken = default );

    Task<ComplaintManagementResult<ComplaintResponse>>
        CreateAsync(
            int actorUserId ,
            CreateComplaintRequest request ,
            CancellationToken cancellationToken = default );
}
