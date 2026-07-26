using Mawasem.Domain.Reviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(
        EntityTypeBuilder<Review> builder )
    {
        builder.ToTable(
            "Reviews" ,
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "CK_Reviews_Rating" ,
                    "[Rating] >= 1 AND [Rating] <= 5");
            });

        builder.HasKey(
            review => review.Id);

        builder.Property(
                review => review.Rating)
            .IsRequired();

        builder.Property(
                review => review.Comment)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(
                review => review.IsVisible)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(
                review => review.ModerationReason)
            .HasMaxLength(500);

        builder.HasOne(
                review => review.Product)
            .WithMany(
                product => product.Reviews)
            .HasForeignKey(
                review => review.ProductId)
            .OnDelete(
                DeleteBehavior.Cascade);

        builder.HasOne(
                review => review.User)
            .WithMany(
                user => user.Reviews)
            .HasForeignKey(
                review => review.UserId)
            .OnDelete(
                DeleteBehavior.Restrict);

        builder.HasOne(
                review => review.ModeratedByEmployee)
            .WithMany()
            .HasForeignKey(
                review => review.ModeratedByEmployeeId)
            .OnDelete(
                DeleteBehavior.Restrict);

        // Multiple reviews from the same customer for the same
        // product are intentionally allowed.
        builder.HasIndex(
            review => new
            {
                review.ProductId ,
                review.UserId
            });

        builder.HasIndex(
            review => new
            {
                review.ProductId ,
                review.IsVisible ,
                review.CreatedOn
            });

        builder.HasIndex(
            review => review.ModeratedByEmployeeId);

        // Soft-deleted reviews are hidden from normal queries.
        builder.HasQueryFilter(
            review => !review.IsDeleted);
    }
}