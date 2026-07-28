using Mawasem.Domain.Common;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Complaints;

public class Complaint : BaseAuditableEntity
{
    public string CustomerName { get; set; } =
        string.Empty;

    public string CustomerPhone { get; set; } =
        string.Empty;

    public string ComplaintText { get; set; } =
        string.Empty;

    public int CreatedByEmployeeId { get; set; }

    public ApplicationUser CreatedByEmployee { get; set; } =
        null!;
}
