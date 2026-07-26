using Mawasem.Domain.Carts;
using Mawasem.Domain.Catalog;
using Mawasem.Domain.Common.ValueObjects;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

internal sealed class CheckoutTestDatabase
    : IAsyncDisposable
{
    internal const int CustomerId = 1;

    internal const int OtherCustomerId = 2;

    internal const int DashboardUserId = 50;

    internal const int DashboardRoleId = 500;

    internal const int ProductId = 100;

    internal const int ProductVariantId = 101;

    internal const int DeliveryAreaId = 200;

    internal const int AddressId = 300;

    internal const int CartId = 400;

    private const string TestActor = "test";

    private readonly string _databasePath;

    private readonly DbContextOptions<MawasemDbContext>
        _options;

    public CheckoutTestDatabase()
    {
        _databasePath =
            Path.Combine(
                Path.GetTempPath() ,
                $"mawasem-checkout-{Guid.NewGuid():N}.db");

        _options =
            new DbContextOptionsBuilder<MawasemDbContext>()
                .UseSqlite(
                    $"Data Source={_databasePath};Foreign Keys=True")
                .Options;

        using var dbContext =
            CreateContext();

        dbContext.Database.EnsureCreated();
    }

    public CheckoutTestDbContext CreateContext()
    {
        return new CheckoutTestDbContext(
            _options);
    }

    public async Task SeedAsync(
        CheckoutSeedOptions? options = null )
    {
        options ??=
            new CheckoutSeedOptions();

        await using var dbContext =
            CreateContext();

        var now =
            DateTimeOffset.UtcNow;

        var customer =
            new ApplicationUser
            {
                Id =
                    CustomerId ,

                UserName =
                    "customer-1" ,

                NormalizedUserName =
                    "CUSTOMER-1" ,

                PhoneNumber =
                    "01000000001" ,

                FullNameAr =
                    "عميل اختبار" ,

                FullNameEn =
                    "Test Customer" ,

                SecurityStamp =
                    Guid.NewGuid().ToString() ,

                IsBlocked =
                    options.CustomerBlocked
            };

        var otherCustomer =
            new ApplicationUser
            {
                Id =
                    OtherCustomerId ,

                UserName =
                    "customer-2" ,

                NormalizedUserName =
                    "CUSTOMER-2" ,

                PhoneNumber =
                    "01000000002" ,

                FullNameAr =
                    "عميل آخر" ,

                FullNameEn =
                    "Other Customer" ,

                SecurityStamp =
                    Guid.NewGuid().ToString()
            };

        var dashboardUser =
            new ApplicationUser
            {
                Id =
                    DashboardUserId ,

                UserName =
                    "dashboard-user" ,

                NormalizedUserName =
                    "DASHBOARD-USER" ,

                PhoneNumber =
                    "01000000050" ,

                FullNameAr =
                    "موظف اختبار" ,

                FullNameEn =
                    "Test Dashboard Employee" ,

                SecurityStamp =
                    Guid.NewGuid().ToString()
            };
        var dashboardRole =
            new ApplicationRole
            {
                Id =
                    DashboardRoleId ,

                Name =
                    SystemRoles.Admin ,

                NormalizedName =
                    SystemRoles.Admin.ToUpperInvariant() ,

                ConcurrencyStamp =
                    Guid.NewGuid().ToString()
            };
        var brand =
            new Brand
            {
                Id =
                    10 ,

                Name =
                    new LocalizedText(
                        "Test Brand" ,
                        "علامة اختبار") ,

                Description =
                    new LocalizedText(
                        "Test brand" ,
                        "علامة اختبار") ,

                IsActive =
                    true ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        var season =
            new Season
            {
                Id =
                    20 ,

                Name =
                    new LocalizedText(
                        "Test Season" ,
                        "موسم اختبار") ,

                Description =
                    new LocalizedText(
                        "Test season" ,
                        "موسم اختبار") ,

                IsActive =
                    true ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        var deliveryArea =
            new DeliveryArea
            {
                Id =
                    DeliveryAreaId ,

                Name =
                    new LocalizedText(
                        "Test Area" ,
                        "منطقة اختبار") ,

                Status =
                    options.DeliveryAreaStatus ,

                DeliveryFee =
                    options.IsFreeDelivery
                        ? 0m
                        : options.DeliveryFee ,

                IsFreeDelivery =
                    options.IsFreeDelivery ,

                IsActive =
                    options.DeliveryAreaActive ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        var product =
            new Product
            {
                Id =
                    ProductId ,

                Name =
                    new LocalizedText(
                        "Test Product" ,
                        "منتج اختبار") ,

                Description =
                    new LocalizedText(
                        "Test product" ,
                        "منتج اختبار") ,

                OriginalPrice =
                    120m ,

                CurrentPrice =
                    options.CurrentPrice ,

                IsPublished =
                    options.ProductPublished ,

                Slug =
                    "test-product" ,

                Brand =
                    brand ,

                Season =
                    season ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        var variant =
            new ProductVariant
            {
                Id =
                    ProductVariantId ,

                Product =
                    product ,

                SKU =
                    "TEST-101" ,

                OptionCombinationKey =
                    "default" ,

                StockQuantity =
                    options.StockQuantity ,

                IsAvailable =
                    options.VariantAvailable ,

                RowVersion =
                    Array.Empty<byte>() ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        product.Variants.Add(
            variant);

        dbContext.Users.AddRange(
            customer ,
            otherCustomer ,
            dashboardUser);

        dbContext.Roles.Add(
            dashboardRole);

        dbContext.UserRoles.Add(
            new IdentityUserRole<int>
            {
                UserId =
                    DashboardUserId ,

                RoleId =
                    DashboardRoleId
            });
        dbContext.DeliveryAreas.Add(
            deliveryArea);

        dbContext.Products.Add(
            product);

        await dbContext.SaveChangesAsync();

        var address =
            new UserAddress
            {
                Id =
                    AddressId ,

                UserId =
                    options.AddressBelongsToOtherCustomer
                        ? OtherCustomerId
                        : CustomerId ,

                DeliveryAreaId =
                    DeliveryAreaId ,

                Label =
                    "Home" ,

                City =
                    "Cairo" ,

                AreaName =
                    "Test Area" ,

                DetailedAddress =
                    "10 Test Street" ,

                BuildingNumber =
                    "10" ,

                FloorNumber =
                    "2" ,

                ApartmentNumber =
                    "5" ,

                Landmark =
                    "Near Test Store" ,

                RecipientName =
                    "Test Recipient" ,

                RecipientPhone =
                    "01000000001" ,

                IsDefault =
                    options.AddressActive ,

                IsActive =
                    options.AddressActive ,

                CreatedOn =
                    now ,

                CreatedBy =
                    TestActor
            };

        dbContext.UserAddresses.Add(
            address);

        if ( options.CreateCart )
        {
            var cart =
                new Cart
                {
                    Id =
                        CartId ,

                    UserId =
                        CustomerId ,

                    CreatedOn =
                        now ,

                    CreatedBy =
                        TestActor
                };

            if ( options.AddCartItem )
            {
                cart.Items.Add(
                    new CartItem
                    {
                        Id =
                            401 ,

                        ProductVariantId =
                            ProductVariantId ,

                        Quantity =
                            options.CartQuantity ,

                        UnitPriceSnapshot =
                            options.PriceSnapshot ,

                        CreatedOn =
                            now ,

                        CreatedBy =
                            TestActor
                    });
            }

            dbContext.Carts.Add(
                cart);
        }

        await dbContext.SaveChangesAsync();
    }

    public ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();

        if ( File.Exists(_databasePath) )
        {
            File.Delete(_databasePath);
        }

        return ValueTask.CompletedTask;
    }
}

internal sealed record CheckoutSeedOptions
{
    public bool CustomerBlocked { get; init; }

    public bool CreateCart { get; init; } =
        true;

    public bool AddCartItem { get; init; } =
        true;

    public bool ProductPublished { get; init; } =
        true;

    public bool VariantAvailable { get; init; } =
        true;

    public int StockQuantity { get; init; } =
        10;

    public int CartQuantity { get; init; } =
        2;

    public decimal CurrentPrice { get; init; } =
        100m;

    public decimal PriceSnapshot { get; init; } =
        100m;

    public bool AddressBelongsToOtherCustomer { get; init; }

    public bool AddressActive { get; init; } =
        true;

    public DeliveryAreaStatus DeliveryAreaStatus { get; init; } =
        DeliveryAreaStatus.Confirmed;

    public bool DeliveryAreaActive { get; init; } =
        true;

    public bool IsFreeDelivery { get; init; }

    public decimal DeliveryFee { get; init; } =
        25m;
}

internal sealed class CheckoutTestDbContext
    : MawasemDbContext
{
    private const string TestVersionProperty =
        "CheckoutTestVersion";

    public CheckoutTestDbContext(
        DbContextOptions<MawasemDbContext> options )
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder builder )
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductVariant>()
            .Property(variant => variant.RowVersion)
            .IsConcurrencyToken(false)
            .ValueGeneratedNever();

        builder.Entity<ProductVariant>()
            .Property<long>(TestVersionProperty)
            .IsConcurrencyToken()
            .HasDefaultValue(0L);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default )
    {
        AdvanceVariantVersions();

        return base.SaveChangesAsync(
            cancellationToken);
    }

    private void AdvanceVariantVersions()
    {
        var modifiedVariants =
            ChangeTracker
                .Entries<ProductVariant>()
                .Where(entry =>
                    entry.State ==
                    EntityState.Modified);

        foreach ( var entry in modifiedVariants )
        {
            var version =
                entry.Property<long>(
                    TestVersionProperty);

            version.CurrentValue =
                version.OriginalValue + 1;
        }
    }
}
