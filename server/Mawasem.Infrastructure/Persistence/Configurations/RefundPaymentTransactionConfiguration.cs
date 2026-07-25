using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class RefundPaymentTransactionConfiguration
    : IEntityTypeConfiguration<RefundPaymentTransaction>
{
    public void Configure(
        EntityTypeBuilder<RefundPaymentTransaction> builder )
    {
        builder.ToTable(
            "RefundPaymentTransactions" ,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_RefundPaymentTransactions_Amount_Positive" ,
                    "[Amount] > 0");
            });

        builder.HasKey(transaction =>
            transaction.Id);

        builder.Property(transaction =>
                transaction.PaymentGateway)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transaction =>
                transaction.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(transaction =>
                transaction.Amount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(transaction =>
                transaction.IdempotencyKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(transaction =>
                transaction.ProviderTransactionId)
            .HasMaxLength(200);

        builder.Property(transaction =>
                transaction.ProviderReference)
            .HasMaxLength(200);

        builder.Property(transaction =>
                transaction.FailureCode)
            .HasMaxLength(100);

        builder.Property(transaction =>
                transaction.FailureMessage)
            .HasMaxLength(2000);

        builder.Property(transaction =>
                transaction.RequestedAt)
            .IsRequired();

        builder.Property(transaction =>
            transaction.CompletedAt);

        builder.Property(transaction =>
            transaction.InitiatedByEmployeeId);

        builder.Property(transaction =>
            transaction.CompletedByEmployeeId);

        builder.HasOne(transaction =>
                transaction.RefundRequest)
            .WithMany(refundRequest =>
                refundRequest.PaymentTransactions)
            .HasForeignKey(transaction =>
                transaction.RefundRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // SQL Server does not allow two SET NULL paths from
        // RefundPaymentTransactions to AspNetUsers.
        builder.HasOne(transaction =>
                transaction.InitiatedByEmployee)
            .WithMany()
            .HasForeignKey(transaction =>
                transaction.InitiatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction =>
                transaction.CompletedByEmployee)
            .WithMany()
            .HasForeignKey(transaction =>
                transaction.CompletedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(transaction =>
            transaction.RefundRequestId);

        builder.HasIndex(transaction =>
            transaction.PaymentGateway);

        builder.HasIndex(transaction =>
            transaction.Status);

        builder.HasIndex(transaction =>
            transaction.RequestedAt);

        builder.HasIndex(transaction =>
            transaction.ProviderTransactionId);

        builder.HasIndex(transaction =>
            transaction.ProviderReference);

        builder.HasIndex(transaction =>
            transaction.InitiatedByEmployeeId);

        builder.HasIndex(transaction =>
            transaction.CompletedByEmployeeId);

        builder.HasIndex(transaction =>
            new
            {
                transaction.RefundRequestId ,
                transaction.Status
            });

        builder.HasIndex(transaction =>
            new
            {
                transaction.RefundRequestId ,
                transaction.IdempotencyKey
            })
            .IsUnique();
    }
}