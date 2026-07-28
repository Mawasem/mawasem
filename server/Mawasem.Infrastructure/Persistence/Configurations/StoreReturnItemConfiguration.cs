using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class StoreReturnItemConfiguration
    : IEntityTypeConfiguration<StoreReturnItem>
{
    public void Configure( EntityTypeBuilder<StoreReturnItem> builder )
    {
        builder.ToTable("StoreReturnItems" , table =>
        {
            table.HasCheckConstraint(
                "CK_StoreReturnItems_Quantity_Positive" ,
                "[Quantity] > 0");

            table.HasCheckConstraint(
                "CK_StoreReturnItems_Amounts_NonNegative" ,
                "[UnitRefundAmount] >= 0 AND " +
                "[TotalRefundAmount] >= 0");
        });

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.UnitRefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(item => item.TotalRefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(item => item.Reason)
            .HasMaxLength(1000);

        builder.HasOne(item => item.StoreReturn)
            .WithMany(returnRequest => returnRequest.Items)
            .HasForeignKey(item => item.StoreReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.OrderItem)
            .WithMany(orderItem => orderItem.StoreReturnItems)
            .HasForeignKey(item => item.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.StoreReturnId);

        builder.HasIndex(item => item.OrderItemId);

        builder.HasIndex(item => new
        {
            item.StoreReturnId ,
            item.OrderItemId
        })
            .IsUnique();
    }
}