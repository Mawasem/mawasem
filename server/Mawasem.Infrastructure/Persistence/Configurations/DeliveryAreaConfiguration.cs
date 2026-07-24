using Mawasem.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public class DeliveryAreaConfiguration
    : IEntityTypeConfiguration<DeliveryArea>
{
    public void Configure( EntityTypeBuilder<DeliveryArea> builder )
    {
        builder.ToTable("DeliveryAreas" , tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_DeliveryAreas_Status" ,
                "[Status] IN (1, 2, 3)");

            tableBuilder.HasCheckConstraint(
                "CK_DeliveryAreas_DeliveryFee_NonNegative" ,
                "[DeliveryFee] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name , nameBuilder =>
        {
            nameBuilder.Property(x => x.English)
                .HasColumnName("NameEnglish")
                .HasMaxLength(200)
                .IsRequired();

            nameBuilder.Property(x => x.Arabic)
                .HasColumnName("NameArabic")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Navigation(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DeliveryFee)
            .HasPrecision(18 , 2)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Property(x => x.IsFreeDelivery)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.Status ,
            x.IsActive
        });
    }
}