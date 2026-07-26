using Mawasem.Domain.Common;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Reviews;

public class Review : BaseAuditableEntity
{
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;

    public string? ModerationReason { get; set; }

    public DateTimeOffset? ModeratedAtUtc { get; set; }

    public int? ModeratedByEmployeeId { get; set; }

    public ApplicationUser? ModeratedByEmployee { get; set; }
}