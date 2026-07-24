using Mawasem.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public class UserAddressConfiguration
    : IEntityTypeConfiguration<UserAddress>
{
    public void Configure( EntityTypeBuilder<UserAddress> builder )
    {
        builder.ToTable("UserAddresses" , tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_UserAddresses_DefaultAddressMustBeActive" ,
                "[IsDefault] = 0 OR [IsActive] = 1");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AreaName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DetailedAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.BuildingNumber)
            .HasMaxLength(50);

        builder.Property(x => x.FloorNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ApartmentNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Landmark)
            .HasMaxLength(300);

        builder.Property(x => x.RecipientName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.RecipientPhone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeliveryArea)
            .WithMany(x => x.UserAddresses)
            .HasForeignKey(x => x.DeliveryAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.DeliveryAreaId);

        builder.HasIndex(x => new
        {
            x.UserId ,
            x.IsActive
        });

        builder.HasIndex(x => new
        {
            x.DeliveryAreaId ,
            x.IsActive
        });

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[IsDefault] = 1 AND [IsActive] = 1")
            .HasDatabaseName(
                "UX_UserAddresses_OneActiveDefaultPerUser");
    }
}