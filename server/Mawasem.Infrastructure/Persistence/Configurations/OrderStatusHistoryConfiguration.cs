using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class OrderStatusHistoryConfiguration
    : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(
        EntityTypeBuilder<OrderStatusHistory> builder )
    {
        builder.ToTable(
            "OrderStatusHistories" ,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_OrderStatusHistories_PreviousStatus" ,
                    "[PreviousStatus] IN " +
                    "(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");

                tableBuilder.HasCheckConstraint(
                    "CK_OrderStatusHistories_NewStatus" ,
                    "[NewStatus] IN " +
                    "(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");

                tableBuilder.HasCheckConstraint(
                    "CK_OrderStatusHistories_StatusChanged" ,
                    "[PreviousStatus] <> [NewStatus]");

                tableBuilder.HasCheckConstraint(
                    "CK_OrderStatusHistories_ActorType" ,
                    "[ActorType] IN (1, 2, 3)");

                tableBuilder.HasCheckConstraint(
                    "CK_OrderStatusHistories_ActorUser" ,
                    "([ActorType] IN (1, 2) AND " +
                    "[ChangedByUserId] IS NOT NULL) OR " +
                    "([ActorType] = 3 AND " +
                    "[ChangedByUserId] IS NULL)");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PreviousStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.NewStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ActorType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ChangedAtUtc)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrderId);

        builder.HasIndex(x => x.ChangedByUserId);

        builder.HasIndex(x => x.NewStatus);

        builder.HasIndex(x => x.ActorType);

        builder.HasIndex(x => x.ChangedAtUtc);

        builder.HasIndex(x => new
        {
            x.OrderId ,
            x.ChangedAtUtc
        });

        builder.HasIndex(x => new
        {
            x.ChangedByUserId ,
            x.ActorType ,
            x.ChangedAtUtc
        });

        builder.HasIndex(x => new
        {
            x.NewStatus ,
            x.ChangedAtUtc
        });
    }
}
