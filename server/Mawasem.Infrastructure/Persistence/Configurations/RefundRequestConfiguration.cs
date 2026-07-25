using Mawasem.Domain.Identity;
using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public class RefundRequestConfiguration
    : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(
        EntityTypeBuilder<RefundRequest> builder )
    {
        builder.ToTable(
            "RefundRequests" ,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RefundRequests_RefundAmount_NonNegative" ,
                    "[RefundAmount] >= 0");
            });

        builder.HasKey(refundRequest =>
            refundRequest.Id);

        builder.Property(refundRequest =>
                refundRequest.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(refundRequest =>
                refundRequest.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(refundRequest =>
                refundRequest.CustomerReason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(refundRequest =>
                refundRequest.AdminNotes)
            .HasMaxLength(2000);

        builder.Property(refundRequest =>
                refundRequest.RefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(refundRequest =>
                refundRequest.RequestedAt)
            .IsRequired();

        builder.Property(refundRequest =>
            refundRequest.ReviewedAt);

        builder.Property(refundRequest =>
            refundRequest.ReviewedByEmployeeId);

        builder.Property(refundRequest =>
            refundRequest.CompletedAt);

        builder.Property(refundRequest =>
            refundRequest.CompletedByEmployeeId);

        builder.Property(refundRequest =>
            refundRequest.StockRestoredAtUtc);

        builder.HasOne(refundRequest =>
                refundRequest.Order)
            .WithMany(order =>
                order.RefundRequests)
            .HasForeignKey(refundRequest =>
                refundRequest.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Both employee relationships use NO ACTION in SQL Server.
        // This avoids multiple cascading SET NULL paths to AspNetUsers.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(refundRequest =>
                refundRequest.ReviewedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(refundRequest =>
                refundRequest.CompletedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(refundRequest =>
            refundRequest.OrderId);

        builder.HasIndex(refundRequest =>
            refundRequest.Status);

        builder.HasIndex(refundRequest =>
            refundRequest.RequestedAt);

        builder.HasIndex(refundRequest =>
            refundRequest.ReviewedByEmployeeId);

        builder.HasIndex(refundRequest =>
            refundRequest.CompletedByEmployeeId);

        builder.HasIndex(refundRequest =>
            refundRequest.CompletedAt);

        builder.HasIndex(refundRequest =>
            refundRequest.StockRestoredAtUtc);

        builder.HasIndex(refundRequest =>
            new
            {
                refundRequest.OrderId ,
                refundRequest.Status
            });

        builder.HasIndex(refundRequest =>
            new
            {
                refundRequest.OrderId ,
                refundRequest.IdempotencyKey
            })
            .IsUnique();
    }
}