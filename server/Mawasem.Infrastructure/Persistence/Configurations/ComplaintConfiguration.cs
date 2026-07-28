using Mawasem.Domain.Complaints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class ComplaintConfiguration
    : IEntityTypeConfiguration<Complaint>
{
    public void Configure(
        EntityTypeBuilder<Complaint> builder )
    {
        builder.ToTable("Complaints");

        builder.HasKey(
            complaint => complaint.Id);

        builder.Property(
                complaint => complaint.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(
                complaint => complaint.CustomerPhone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(
                complaint => complaint.ComplaintText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(
                complaint => complaint.CreatedByEmployeeId)
            .IsRequired();

        builder.HasOne(
                complaint => complaint.CreatedByEmployee)
            .WithMany()
            .HasForeignKey(
                complaint => complaint.CreatedByEmployeeId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasIndex(
            complaint => complaint.CustomerPhone);

        builder.HasIndex(
            complaint => complaint.CreatedByEmployeeId);

        builder.HasIndex(
            complaint => complaint.CreatedOn);

        builder.HasIndex(
            complaint => new
            {
                complaint.CreatedByEmployeeId ,
                complaint.CreatedOn
            });

        builder.HasQueryFilter(
            complaint => !complaint.IsDeleted);
    }
}
