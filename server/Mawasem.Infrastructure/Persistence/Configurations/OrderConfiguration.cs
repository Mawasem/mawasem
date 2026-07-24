using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mawasem.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure( EntityTypeBuilder<Order> builder )
    {
        builder.ToTable("Orders" , tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "CK_Orders_Amounts_NonNegative" ,
                "[SubTotal] >= 0 AND " +
                "[Discount] >= 0 AND " +
                "[DeliveryFee] >= 0 AND " +
                "[TotalAmount] >= 0");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_Discount_NotGreaterThan_SubTotal" ,
                "[Discount] <= [SubTotal]");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_OrderStatus" ,
                "[OrderStatus] IN (1, 2, 3, 4, 5, 6, 7, 8, 9)");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_PaymentMethod" ,
                "[PaymentMethod] IN (1, 2)");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_PaymentStatus" ,
                "[PaymentStatus] IN (1, 2, 3, 4)");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_DeliveryMethod" ,
                "[DeliveryMethod] IN (1, 2)");

            tableBuilder.HasCheckConstraint(
                "CK_Orders_OrderSource" ,
                "[OrderSource] IN (1, 2)");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(128);

        builder.HasIndex(x => new
        {
            x.UserId ,
            x.IdempotencyKey
        })
            .IsUnique()
            .HasFilter("[IdempotencyKey] IS NOT NULL");

        builder.Property(x => x.CustomerNameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerNameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ShippingRecipientName)
            .HasMaxLength(200);

        builder.Property(x => x.ShippingRecipientPhone)
            .HasMaxLength(30);

        builder.Property(x => x.ShippingCity)
            .HasMaxLength(100);

        builder.Property(x => x.ShippingAreaName)
            .HasMaxLength(200);

        builder.Property(x => x.ShippingDetailedAddress)
            .HasMaxLength(500);

        builder.Property(x => x.ShippingBuildingNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ShippingFloorNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ShippingApartmentNumber)
            .HasMaxLength(50);

        builder.Property(x => x.ShippingLandmark)
            .HasMaxLength(300);

        builder.Property(x => x.ShippingDeliveryAreaNameAr)
            .HasMaxLength(200);

        builder.Property(x => x.ShippingDeliveryAreaNameEn)
            .HasMaxLength(200);

        builder.Property(x => x.OrderDate)
            .IsRequired();

        builder.Property(x => x.SubTotal)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(x => x.Discount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(x => x.DeliveryFee)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18 , 2)
            .IsRequired();

        builder.Property(x => x.CouponCode)
            .HasMaxLength(100);

        builder.Property(x => x.OrderStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PaymentMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DeliveryMethod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.OrderSource)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserAddress)
            .WithMany()
            .HasForeignKey(x => x.UserAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShippingDeliveryArea)
            .WithMany()
            .HasForeignKey(x => x.ShippingDeliveryAreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.UserAddressId);

        builder.HasIndex(x => x.ShippingDeliveryAreaId);

        builder.HasIndex(x => x.OrderDate);

        builder.HasIndex(x => x.OrderStatus);

        builder.HasIndex(x => x.PaymentStatus);

        builder.HasIndex(x => x.PaymentMethod);

        builder.HasIndex(x => new
        {
            x.UserId ,
            x.OrderDate
        });

        builder.HasIndex(x => new
        {
            x.OrderStatus ,
            x.OrderDate
        });
    }
}