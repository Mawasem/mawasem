using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public class RefundRequestItemConfiguration
    : IEntityTypeConfiguration<RefundRequestItem>
{
    public void Configure(
        EntityTypeBuilder<RefundRequestItem> builder )
    {
        builder.ToTable(
            "RefundRequestItems" ,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequestItems_Quantity_Positive" ,
                    "[Quantity] > 0");

                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequestItems_ReturnedQuantity_Valid" ,
                    "[ReturnedQuantity] >= 0 AND " +
                    "[ReturnedQuantity] <= [Quantity]");

                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequestItems_RestockQuantity_Valid" ,
                    "[RestockQuantity] >= 0 AND " +
                    "[RestockQuantity] <= [ReturnedQuantity]");

                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequestItems_UnitRefundAmount_NonNegative" ,
                    "[UnitRefundAmount] >= 0");

                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequestItems_TotalRefundAmount_NonNegative" ,
                    "[TotalRefundAmount] >= 0");
            });

        builder.HasKey(refundRequestItem =>
            refundRequestItem.Id);

        builder.Property(refundRequestItem =>
                refundRequestItem.Quantity)
            .IsRequired();

        builder.Property(refundRequestItem =>
                refundRequestItem.ReturnedQuantity)
            .IsRequired();

        builder.Property(refundRequestItem =>
                refundRequestItem.RestockQuantity)
            .IsRequired();

        builder.Property(refundRequestItem =>
                refundRequestItem.Reason)
            .HasMaxLength(1000);

        builder.Property(refundRequestItem =>
                refundRequestItem.UnitRefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(refundRequestItem =>
                refundRequestItem.TotalRefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.HasOne(refundRequestItem =>
                refundRequestItem.RefundRequest)
            .WithMany(refundRequest =>
                refundRequest.Items)
            .HasForeignKey(refundRequestItem =>
                refundRequestItem.RefundRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(refundRequestItem =>
                refundRequestItem.OrderItem)
            .WithMany()
            .HasForeignKey(refundRequestItem =>
                refundRequestItem.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(refundRequestItem =>
            refundRequestItem.RefundRequestId);

        builder.HasIndex(refundRequestItem =>
            refundRequestItem.OrderItemId);

        builder.HasIndex(refundRequestItem =>
            new
            {
                refundRequestItem.RefundRequestId ,
                refundRequestItem.OrderItemId
            })
            .IsUnique();
    }
}