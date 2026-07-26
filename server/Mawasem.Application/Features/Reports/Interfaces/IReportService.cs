using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Contracts.Responses;
using Mawasem.Application.Features.Reports.Models;

namespace Mawasem.Application.Features.Reports.Interfaces;

public interface IReportService
{
    Task<ReportResult<EmployeeReportSummaryResponse>>
        GetEmployeeSummaryAsync(
            GetEmployeeReportRequest request ,
            CancellationToken cancellationToken = default );

    Task<ReportResult<EmployeeOrderActionsResponse>>
        GetEmployeeOrderActionsAsync(
            int employeeId ,
            GetEmployeeOrderActionsRequest request ,
            CancellationToken cancellationToken = default );
}
