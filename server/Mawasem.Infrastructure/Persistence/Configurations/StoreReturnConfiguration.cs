using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class StoreReturnConfiguration
    : IEntityTypeConfiguration<StoreReturn>
{
    public void Configure( EntityTypeBuilder<StoreReturn> builder )
    {
        builder.ToTable("StoreReturns" , table =>
        {
            table.HasCheckConstraint(
                "CK_StoreReturns_TotalRefundAmount_NonNegative" ,
                "[TotalRefundAmount] >= 0");

            table.HasCheckConstraint(
                "CK_StoreReturns_RefundPaymentMethod" ,
                "[RefundPaymentMethod] IN (3, 4, 5)");

            table.HasCheckConstraint(
                "CK_StoreReturns_PhysicalPaymentReference" ,
                "[RefundPaymentMethod] NOT IN (4, 5) OR " +
                "([RefundPaymentReference] IS NOT NULL AND " +
                "LTRIM(RTRIM([RefundPaymentReference])) <> '')");
        });

        builder.HasKey(returnRequest => returnRequest.Id);

        builder.Property(returnRequest => returnRequest.ReturnNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(returnRequest => returnRequest.ReturnNumber)
            .IsUnique();

        builder.Property(returnRequest => returnRequest.RefundPaymentMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(returnRequest => returnRequest.RefundPaymentReference)
            .HasMaxLength(200);

        builder.Property(returnRequest => returnRequest.TotalRefundAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(returnRequest => returnRequest.ReturnedAtUtc)
            .IsRequired();

        builder.HasOne(returnRequest => returnRequest.Order)
            .WithMany(order => order.StoreReturns)
            .HasForeignKey(returnRequest => returnRequest.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(returnRequest => returnRequest.ProcessedByEmployee)
            .WithMany()
            .HasForeignKey(returnRequest => returnRequest.ProcessedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(returnRequest => returnRequest.OrderId);

        builder.HasIndex(returnRequest => returnRequest.ProcessedByEmployeeId);

        builder.HasIndex(returnRequest => returnRequest.ReturnedAtUtc);
    }
}