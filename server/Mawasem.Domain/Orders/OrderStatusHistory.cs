using Mawasem.Domain.Common;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Orders;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public OrderStatus PreviousStatus { get; set; }

    public OrderStatus NewStatus { get; set; }

    public int? ChangedByUserId { get; set; }

    public ApplicationUser? ChangedByUser { get; set; }

    public OrderStatusChangeActorType ActorType { get; set; }

    public DateTime ChangedAtUtc { get; set; } =
        DateTime.UtcNow;

    public string? Reason { get; set; }
}