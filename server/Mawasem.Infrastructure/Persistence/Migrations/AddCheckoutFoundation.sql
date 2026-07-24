IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [FullNameAr] nvarchar(max) NOT NULL,
        [FullNameEn] nvarchar(max) NOT NULL,
        [BirthDate] date NULL,
        [Gender] int NOT NULL,
        [ReferralSource] int NOT NULL,
        [IsBlocked] bit NOT NULL,
        [BlockedAt] datetime2 NULL,
        [BlockedReason] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Brands] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [DescriptionEn] nvarchar(500) NOT NULL,
        [DescriptionAr] nvarchar(500) NOT NULL,
        [LogoUrl] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Brands] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Collections] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Collections] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [DeliveryAreas] (
        [Id] int NOT NULL IDENTITY,
        [NameEnglish] nvarchar(200) NOT NULL,
        [NameArabic] nvarchar(200) NOT NULL,
        [DeliveryFee] decimal(18,2) NOT NULL DEFAULT 0.0,
        [IsFreeDelivery] bit NOT NULL DEFAULT CAST(0 AS bit),
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_DeliveryAreas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Grades] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Grades] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Permissions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductOptions] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductOptions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Seasons] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [DescriptionEn] nvarchar(500) NOT NULL,
        [DescriptionAr] nvarchar(500) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Seasons] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Tags] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [UserAddresses] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Label] nvarchar(max) NOT NULL,
        [City] nvarchar(max) NOT NULL,
        [AreaName] nvarchar(max) NOT NULL,
        [DetailedAddress] nvarchar(max) NOT NULL,
        [BuildingNumber] nvarchar(max) NULL,
        [FloorNumber] nvarchar(max) NULL,
        [ApartmentNumber] nvarchar(max) NULL,
        [Landmark] nvarchar(max) NULL,
        [DeliveryAreaId] int NULL,
        [CustomAreaName] nvarchar(max) NULL,
        [RequiresAreaReview] bit NOT NULL,
        [RecipientName] nvarchar(max) NOT NULL,
        [RecipientPhone] nvarchar(max) NOT NULL,
        [IsDefault] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_UserAddresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserAddresses_DeliveryAreas_DeliveryAreaId] FOREIGN KEY ([DeliveryAreaId]) REFERENCES [DeliveryAreas] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [RolePermissions] (
        [RoleId] int NOT NULL,
        [PermissionId] int NOT NULL,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductOptionValues] (
        [Id] int NOT NULL IDENTITY,
        [ProductOptionId] int NOT NULL,
        [ValueEn] nvarchar(100) NOT NULL,
        [ValueAr] nvarchar(100) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductOptionValues] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductOptionValues_ProductOptions_ProductOptionId] FOREIGN KEY ([ProductOptionId]) REFERENCES [ProductOptions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [NameEn] nvarchar(200) NOT NULL,
        [NameAr] nvarchar(200) NOT NULL,
        [DescriptionEn] nvarchar(2000) NOT NULL,
        [DescriptionAr] nvarchar(2000) NOT NULL,
        [OriginalPrice] decimal(18,2) NOT NULL,
        [CurrentPrice] decimal(18,2) NOT NULL,
        [IsPublished] bit NOT NULL,
        [IsFeatured] bit NOT NULL,
        [Slug] nvarchar(300) NOT NULL,
        [BrandId] int NOT NULL,
        [SeasonId] int NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Products_Brands_BrandId] FOREIGN KEY ([BrandId]) REFERENCES [Brands] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Products_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Orders] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [CustomerNameAr] nvarchar(200) NOT NULL,
        [CustomerNameEn] nvarchar(200) NOT NULL,
        [CustomerPhone] nvarchar(20) NOT NULL,
        [UserAddressId] int NULL,
        [OrderNumber] nvarchar(50) NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        [Discount] decimal(18,2) NOT NULL,
        [DeliveryFee] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [CouponCode] nvarchar(100) NULL,
        [OrderStatus] int NOT NULL,
        [PaymentStatus] int NOT NULL,
        [DeliveryMethod] int NOT NULL,
        [OrderSource] int NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CancellationReason] nvarchar(500) NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Orders_UserAddresses_UserAddressId] FOREIGN KEY ([UserAddressId]) REFERENCES [UserAddresses] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductCategories] (
        [ProductId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([ProductId], [CategoryId]),
        CONSTRAINT [FK_ProductCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductCategories_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductCollections] (
        [ProductId] int NOT NULL,
        [CollectionId] int NOT NULL,
        CONSTRAINT [PK_ProductCollections] PRIMARY KEY ([ProductId], [CollectionId]),
        CONSTRAINT [FK_ProductCollections_Collections_CollectionId] FOREIGN KEY ([CollectionId]) REFERENCES [Collections] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductCollections_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductGrades] (
        [ProductId] int NOT NULL,
        [GradeId] int NOT NULL,
        CONSTRAINT [PK_ProductGrades] PRIMARY KEY ([ProductId], [GradeId]),
        CONSTRAINT [FK_ProductGrades_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Grades] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductGrades_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductSpecifications] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [NameEn] nvarchar(100) NOT NULL,
        [NameAr] nvarchar(100) NOT NULL,
        [ValueEn] nvarchar(500) NOT NULL,
        [ValueAr] nvarchar(500) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductSpecifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductSpecifications_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductTags] (
        [ProductId] int NOT NULL,
        [TagId] int NOT NULL,
        CONSTRAINT [PK_ProductTags] PRIMARY KEY ([ProductId], [TagId]),
        CONSTRAINT [FK_ProductTags_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductTags_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductVariants] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [SKU] nvarchar(100) NOT NULL,
        [StockQuantity] int NOT NULL,
        [IsAvailable] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductVariants] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductVariants_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [UserId] int NOT NULL,
        [Rating] int NOT NULL,
        [Comment] nvarchar(1000) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [RefundRequests] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [Status] int NOT NULL,
        [CustomerReason] nvarchar(1000) NULL,
        [AdminNotes] nvarchar(2000) NULL,
        [RequestedAt] datetime2 NOT NULL,
        [ReviewedAt] datetime2 NULL,
        [ReviewedByEmployeeId] int NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_RefundRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefundRequests_AspNetUsers_ReviewedByEmployeeId] FOREIGN KEY ([ReviewedByEmployeeId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_RefundRequests_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [OrderItems] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ProductVariantId] int NOT NULL,
        [ProductNameAr] nvarchar(200) NOT NULL,
        [ProductNameEn] nvarchar(200) NOT NULL,
        [SKU] nvarchar(100) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0.0,
        [Quantity] int NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [RefundedQuantity] int NOT NULL DEFAULT 0,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductImages] (
        [Id] int NOT NULL IDENTITY,
        [ProductVariantId] int NOT NULL,
        [ImageUrl] nvarchar(500) NOT NULL,
        [IsPrimary] bit NOT NULL DEFAULT CAST(0 AS bit),
        [DisplayOrder] int NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductImages_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [ProductVariantOptions] (
        [Id] int NOT NULL IDENTITY,
        [ProductVariantId] int NOT NULL,
        [ProductOptionValueId] int NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_ProductVariantOptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProductVariantOptions_ProductOptionValues_ProductOptionValueId] FOREIGN KEY ([ProductOptionValueId]) REFERENCES [ProductOptionValues] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProductVariantOptions_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE TABLE [RefundRequestItems] (
        [Id] int NOT NULL IDENTITY,
        [RefundRequestId] int NOT NULL,
        [OrderItemId] int NOT NULL,
        [Quantity] int NOT NULL,
        [Reason] nvarchar(1000) NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_RefundRequestItems] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_RefundRequestItems_Quantity_Positive] CHECK ([Quantity] > 0),
        CONSTRAINT [FK_RefundRequestItems_OrderItems_OrderItemId] FOREIGN KEY ([OrderItemId]) REFERENCES [OrderItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RefundRequestItems_RefundRequests_RefundRequestId] FOREIGN KEY ([RefundRequestId]) REFERENCES [RefundRequests] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_DeliveryAreas_IsActive] ON [DeliveryAreas] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_OrderItems_OrderId_ProductVariantId] ON [OrderItems] ([OrderId], [ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_OrderItems_ProductVariantId] ON [OrderItems] ([ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_OrderItems_SKU] ON [OrderItems] ([SKU]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_OrderDate] ON [Orders] ([OrderDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Orders_OrderNumber] ON [Orders] ([OrderNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_OrderStatus] ON [Orders] ([OrderStatus]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_PaymentStatus] ON [Orders] ([PaymentStatus]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_UserAddressId] ON [Orders] ([UserAddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Orders_UserId_OrderDate] ON [Orders] ([UserId], [OrderDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permissions_Name] ON [Permissions] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductCategories_CategoryId] ON [ProductCategories] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductCollections_CollectionId] ON [ProductCollections] ([CollectionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductGrades_GradeId] ON [ProductGrades] ([GradeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductImages_ProductVariantId] ON [ProductImages] ([ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductOptionValues_ProductOptionId] ON [ProductOptionValues] ([ProductOptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Products_BrandId] ON [Products] ([BrandId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Products_SeasonId] ON [Products] ([SeasonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_Slug] ON [Products] ([Slug]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductSpecifications_ProductId] ON [ProductSpecifications] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductTags_TagId] ON [ProductTags] ([TagId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductVariantOptions_ProductOptionValueId] ON [ProductVariantOptions] ([ProductOptionValueId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductVariantOptions_ProductVariantId] ON [ProductVariantOptions] ([ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_ProductVariants_ProductId] ON [ProductVariants] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductVariants_SKU] ON [ProductVariants] ([SKU]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequestItems_OrderItemId] ON [RefundRequestItems] ([OrderItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequestItems_RefundRequestId] ON [RefundRequestItems] ([RefundRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefundRequestItems_RefundRequestId_OrderItemId] ON [RefundRequestItems] ([RefundRequestId], [OrderItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_OrderId] ON [RefundRequests] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_OrderId_Status] ON [RefundRequests] ([OrderId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_RequestedAt] ON [RefundRequests] ([RequestedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_ReviewedByEmployeeId] ON [RefundRequests] ([ReviewedByEmployeeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RefundRequests_Status] ON [RefundRequests] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_ProductId_UserId] ON [Reviews] ([ProductId], [UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_UserAddresses_DeliveryAreaId] ON [UserAddresses] ([DeliveryAreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    CREATE INDEX [IX_UserAddresses_UserId] ON [UserAddresses] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260710142054_AddOrderModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260710142054_AddOrderModule', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    DROP INDEX [IX_Reviews_ProductId_UserId] ON [Reviews];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE TABLE [PasswordResetCodes] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Channel] int NOT NULL,
        [CodeHash] nvarchar(128) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [VerifiedAtUtc] datetime2 NULL,
        [UsedAtUtc] datetime2 NULL,
        [RevokedAtUtc] datetime2 NULL,
        [FailedAttempts] int NOT NULL,
        [RequestedByIp] nvarchar(45) NULL,
        CONSTRAINT [PK_PasswordResetCodes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PasswordResetCodes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(128) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [CreatedByIp] nvarchar(45) NULL,
        [RevokedAtUtc] datetime2 NULL,
        [RevokedByIp] nvarchar(45) NULL,
        [ReplacedByTokenHash] nvarchar(128) NULL,
        [RevocationReason] nvarchar(256) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE INDEX [IX_Reviews_ProductId_UserId] ON [Reviews] ([ProductId], [UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE INDEX [IX_PasswordResetCodes_ExpiresAtUtc] ON [PasswordResetCodes] ([ExpiresAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE INDEX [IX_PasswordResetCodes_UserId_Channel_ExpiresAtUtc] ON [PasswordResetCodes] ([UserId], [Channel], [ExpiresAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId_ExpiresAtUtc] ON [RefreshTokens] ([UserId], [ExpiresAtUtc]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711211214_AddAuthenticationAndReviewUpdates'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260711211214_AddAuthenticationAndReviewUpdates', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumber');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [PhoneNumber] nvarchar(450) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'IsBlocked');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [AspNetUsers] ADD DEFAULT CAST(0 AS bit) FOR [IsBlocked];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FullNameEn');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [FullNameEn] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FullNameAr');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [FullNameAr] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'BlockedReason');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [BlockedReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_IsBlocked] ON [AspNetUsers] ([IsBlocked]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_PhoneNumber] ON [AspNetUsers] ([PhoneNumber]) WHERE [PhoneNumber] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260711220247_AddUniqueUserPhoneNumber'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260711220247_AddUniqueUserPhoneNumber', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713075432_AddDashboardAuthenticationFoundation'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ReferralSource');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [ReferralSource] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713075432_AddDashboardAuthenticationFoundation'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Gender');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [Gender] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713075432_AddDashboardAuthenticationFoundation'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MustChangePassword] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713075432_AddDashboardAuthenticationFoundation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260713075432_AddDashboardAuthenticationFoundation', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713212750_AddCustomerPasswordResetToken'
)
BEGIN
    ALTER TABLE [PasswordResetCodes] ADD [ResetTokenExpiresAtUtc] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713212750_AddCustomerPasswordResetToken'
)
BEGIN
    ALTER TABLE [PasswordResetCodes] ADD [ResetTokenHash] nvarchar(128) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713212750_AddCustomerPasswordResetToken'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PasswordResetCodes_ResetTokenHash] ON [PasswordResetCodes] ([ResetTokenHash]) WHERE [ResetTokenHash] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260713212750_AddCustomerPasswordResetToken'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260713212750_AddCustomerPasswordResetToken', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714085930_AddUserPermissions'
)
BEGIN
    CREATE TABLE [UserPermissions] (
        [UserId] int NOT NULL,
        [PermissionId] int NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([UserId], [PermissionId]),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714085930_AddUserPermissions'
)
BEGIN
    CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714085930_AddUserPermissions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260714085930_AddUserPermissions', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260717183639_AddSeasonActiveStatus'
)
BEGIN
    ALTER TABLE [Seasons] ADD [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260717183639_AddSeasonActiveStatus'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260717183639_AddSeasonActiveStatus', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    ALTER TABLE [ProductVariantOptions] DROP CONSTRAINT [FK_ProductVariantOptions_ProductOptionValues_ProductOptionValueId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    DROP INDEX [IX_ProductVariants_ProductId] ON [ProductVariants];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    DROP INDEX [IX_ProductVariantOptions_ProductVariantId] ON [ProductVariantOptions];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    ALTER TABLE [ProductVariants] ADD [OptionCombinationKey] nvarchar(450) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    ALTER TABLE [ProductVariants] ADD [RowVersion] rowversion NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    UPDATE [pv]
    SET [pv].[OptionCombinationKey] =
        COALESCE(
            STUFF(
                (
                    SELECT
                        N'|' +
                        CONVERT(
                            nvarchar(20),
                            [pvo].[ProductOptionValueId])
                    FROM [ProductVariantOptions] AS [pvo]
                    WHERE
                        [pvo].[ProductVariantId] =
                        [pv].[Id]
                    ORDER BY
                        [pvo].[ProductOptionValueId]
                    FOR XML PATH(''), TYPE
                ).value(
                    '.',
                    'nvarchar(max)'),
                1,
                1,
                N''),
            N'DEFAULT')
    FROM [ProductVariants] AS [pv];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProductVariants]') AND [c].[name] = N'OptionCombinationKey');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ProductVariants] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [ProductVariants] ALTER COLUMN [OptionCombinationKey] nvarchar(450) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM [ProductVariants]
        GROUP BY
            [ProductId],
            [OptionCombinationKey]
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 50001,
            'Existing product variants contain duplicate option combinations for the same product.',
            1;
    END;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductVariants_ProductId_OptionCombinationKey] ON [ProductVariants] ([ProductId], [OptionCombinationKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    EXEC(N'ALTER TABLE [ProductVariants] ADD CONSTRAINT [CK_ProductVariants_StockQuantity_NonNegative] CHECK ([StockQuantity] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductVariantOptions_ProductVariantId_ProductOptionValueId] ON [ProductVariantOptions] ([ProductVariantId], [ProductOptionValueId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    ALTER TABLE [ProductVariantOptions] ADD CONSTRAINT [FK_ProductVariantOptions_ProductOptionValues_ProductOptionValueId] FOREIGN KEY ([ProductOptionValueId]) REFERENCES [ProductOptionValues] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721093308_AddProductVariantStockManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260721093308_AddProductVariantStockManagement', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductImages] DROP CONSTRAINT [FK_ProductImages_ProductVariants_ProductVariantId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    DROP INDEX [IX_ProductImages_ProductVariantId] ON [ProductImages];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    EXEC sp_rename N'[ProductImages].[ProductVariantId]', N'ProductId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductOptions] ADD [Type] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductImages] ADD [ColorOptionValueId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductImages] ADD [StorageKey] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_ProductOptions_SingleColorOption] ON [ProductOptions] ([Type]) WHERE [Type] = 2');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    CREATE INDEX [IX_ProductImages_ColorOptionValueId] ON [ProductImages] ([ColorOptionValueId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ProductImages_StorageKey] ON [ProductImages] ([StorageKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    CREATE UNIQUE INDEX [UX_ProductImages_GalleryDisplayOrder] ON [ProductImages] ([ProductId], [ColorOptionValueId], [DisplayOrder]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_ProductImages_GalleryPrimary] ON [ProductImages] ([ProductId], [ColorOptionValueId]) WHERE [IsPrimary] = 1');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    EXEC(N'ALTER TABLE [ProductImages] ADD CONSTRAINT [CK_ProductImages_DisplayOrder_NonNegative] CHECK ([DisplayOrder] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductImages] ADD CONSTRAINT [FK_ProductImages_ProductOptionValues_ColorOptionValueId] FOREIGN KEY ([ColorOptionValueId]) REFERENCES [ProductOptionValues] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    ALTER TABLE [ProductImages] ADD CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721172449_AddProductImageManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260721172449_AddProductImageManagement', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721201331_AddSeasonToCollections'
)
BEGIN
    ALTER TABLE [Collections] ADD [SeasonId] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721201331_AddSeasonToCollections'
)
BEGIN
    CREATE INDEX [IX_Collections_SeasonId] ON [Collections] ([SeasonId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721201331_AddSeasonToCollections'
)
BEGIN
    ALTER TABLE [Collections] ADD CONSTRAINT [FK_Collections_Seasons_SeasonId] FOREIGN KEY ([SeasonId]) REFERENCES [Seasons] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260721201331_AddSeasonToCollections'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260721201331_AddSeasonToCollections', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722134235_AddPendingProductImageDeletion'
)
BEGIN
    CREATE TABLE [PendingProductImageDeletions] (
        [Id] int NOT NULL IDENTITY,
        [StorageKey] nvarchar(500) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [AttemptCount] int NOT NULL,
        [NextAttemptAt] datetimeoffset NOT NULL,
        [LastAttemptAt] datetimeoffset NULL,
        [LastError] nvarchar(2000) NULL,
        CONSTRAINT [PK_PendingProductImageDeletions] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_PendingProductImageDeletions_AttemptCount] CHECK ([AttemptCount] >= 0)
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722134235_AddPendingProductImageDeletion'
)
BEGIN
    CREATE INDEX [IX_PendingProductImageDeletions_NextAttemptAt_Id] ON [PendingProductImageDeletions] ([NextAttemptAt], [Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722134235_AddPendingProductImageDeletion'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PendingProductImageDeletions_StorageKey] ON [PendingProductImageDeletions] ([StorageKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722134235_AddPendingProductImageDeletion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260722134235_AddPendingProductImageDeletion', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    CREATE TABLE [Carts] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NULL,
        [GuestTokenHash] char(64) NULL,
        [GuestExpiresOn] datetimeoffset NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Carts_Owner] CHECK ((
        [UserId] IS NOT NULL
        AND [GuestTokenHash] IS NULL
        AND [GuestExpiresOn] IS NULL
    )
    OR
    (
        [UserId] IS NULL
        AND [GuestTokenHash] IS NOT NULL
        AND [GuestExpiresOn] IS NOT NULL
    )),
        CONSTRAINT [FK_Carts_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    CREATE TABLE [CartItems] (
        [Id] int NOT NULL IDENTITY,
        [CartId] int NOT NULL,
        [ProductVariantId] int NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPriceSnapshot] decimal(18,2) NOT NULL,
        [CreatedOn] datetimeoffset NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedOn] datetimeoffset NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedOn] datetimeoffset NULL,
        [DeletedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_CartItems_Quantity] CHECK ([Quantity] > 0),
        CONSTRAINT [CK_CartItems_UnitPriceSnapshot] CHECK ([UnitPriceSnapshot] >= 0),
        CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CartItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CartItems_CartId_ProductVariantId] ON [CartItems] ([CartId], [ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    CREATE INDEX [IX_CartItems_ProductVariantId] ON [CartItems] ([ProductVariantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    CREATE INDEX [IX_Carts_GuestExpiresOn] ON [Carts] ([GuestExpiresOn]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Carts_GuestTokenHash] ON [Carts] ([GuestTokenHash]) WHERE [GuestTokenHash] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260722184842_AddShoppingCartModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260722184842_AddShoppingCartModule', N'8.0.28');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [UserAddresses] DROP CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [UserAddresses] DROP CONSTRAINT [FK_UserAddresses_DeliveryAreas_DeliveryAreaId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DROP INDEX [IX_UserAddresses_UserId] ON [UserAddresses];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [CancelledAtUtc] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [IdempotencyKey] nvarchar(128) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [PaymentMethod] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [RejectedAtUtc] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [RejectionReason] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingApartmentNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingAreaName] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingBuildingNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingCity] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingDeliveryAreaId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingDeliveryAreaNameAr] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingDeliveryAreaNameEn] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingDetailedAddress] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingFloorNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingLandmark] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingRecipientName] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [ShippingRecipientPhone] nvarchar(30) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD [StockRestoredAtUtc] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [ProductId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [VariantSummaryAr] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [OrderItems] ADD [VariantSummaryEn] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [DeliveryAreas] ADD [Status] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    UPDATE [DeliveryAreas]
    SET [Status] = 2
    WHERE [Status] IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ;WITH [LegacyAreaNames] AS
    (
        SELECT DISTINCT
            LEFT(
                LTRIM(RTRIM([CustomAreaName])),
                200) AS [AreaName]
        FROM [UserAddresses]
        WHERE [DeliveryAreaId] IS NULL
          AND NULLIF(
                LTRIM(RTRIM([CustomAreaName])),
                N'') IS NOT NULL
    )
    INSERT INTO [DeliveryAreas]
    (
        [NameEnglish],
        [NameArabic],
        [Status],
        [DeliveryFee],
        [IsFreeDelivery],
        [IsActive],
        [CreatedOn],
        [CreatedBy],
        [IsDeleted]
    )
    SELECT
        [legacy].[AreaName],
        [legacy].[AreaName],
        1,
        0,
        0,
        1,
        SYSDATETIMEOFFSET(),
        N'checkout-migration',
        0
    FROM [LegacyAreaNames] AS [legacy]
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [DeliveryAreas] AS [area]
        WHERE [area].[IsDeleted] = 0
          AND
          (
              [area].[NameEnglish] =
                  [legacy].[AreaName]
              OR
              [area].[NameArabic] =
                  [legacy].[AreaName]
          )
    );

    UPDATE [address]
    SET [DeliveryAreaId] =
        [matchingArea].[Id]
    FROM [UserAddresses] AS [address]
    CROSS APPLY
    (
        SELECT TOP (1)
            [area].[Id]
        FROM [DeliveryAreas] AS [area]
        WHERE [area].[IsDeleted] = 0
          AND
          (
              [area].[NameEnglish] =
                  LEFT(
                      LTRIM(
                          RTRIM(
                              [address].[CustomAreaName])),
                      200)
              OR
              [area].[NameArabic] =
                  LEFT(
                      LTRIM(
                          RTRIM(
                              [address].[CustomAreaName])),
                      200)
          )
        ORDER BY
            CASE
                WHEN [area].[Status] = 2
                    THEN 0
                ELSE 1
            END,
            [area].[Id]
    ) AS [matchingArea]
    WHERE [address].[DeliveryAreaId] IS NULL
      AND NULLIF(
            LTRIM(
                RTRIM(
                    [address].[CustomAreaName])),
            N'') IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM [UserAddresses]
        WHERE [DeliveryAreaId] IS NULL
    )
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1
            FROM [DeliveryAreas]
            WHERE [NameEnglish] =
                    N'Legacy Unassigned Area'
              AND [NameArabic] =
                    N'منطقة قديمة غير محددة'
              AND [IsDeleted] = 0
        )
        BEGIN
            INSERT INTO [DeliveryAreas]
            (
                [NameEnglish],
                [NameArabic],
                [Status],
                [DeliveryFee],
                [IsFreeDelivery],
                [IsActive],
                [CreatedOn],
                [CreatedBy],
                [IsDeleted]
            )
            VALUES
            (
                N'Legacy Unassigned Area',
                N'منطقة قديمة غير محددة',
                1,
                0,
                0,
                1,
                SYSDATETIMEOFFSET(),
                N'checkout-migration',
                0
            );
        END;

        DECLARE @LegacyAreaId int;

        SELECT TOP (1)
            @LegacyAreaId = [Id]
        FROM [DeliveryAreas]
        WHERE [NameEnglish] =
                N'Legacy Unassigned Area'
          AND [NameArabic] =
                N'منطقة قديمة غير محددة'
          AND [IsDeleted] = 0
        ORDER BY [Id];

        UPDATE [UserAddresses]
        SET [DeliveryAreaId] =
            @LegacyAreaId
        WHERE [DeliveryAreaId] IS NULL;
    END;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    UPDATE [item]
    SET [ProductId] =
        [variant].[ProductId]
    FROM [OrderItems] AS [item]
    INNER JOIN [ProductVariants] AS [variant]
        ON [variant].[Id] =
            [item].[ProductVariantId];

    IF EXISTS
    (
        SELECT 1
        FROM [OrderItems]
        WHERE [ProductId] IS NULL
    )
    BEGIN
        THROW 51000,
            'Cannot migrate OrderItems because a ProductVariant reference is invalid.',
            1;
    END;

    UPDATE [OrderItems]
    SET
        [VariantSummaryAr] = N'',
        [VariantSummaryEn] = N''
    WHERE [VariantSummaryAr] IS NULL
       OR [VariantSummaryEn] IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    UPDATE [Orders]
    SET [PaymentMethod] = 1
    WHERE [PaymentMethod] IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    UPDATE [order]
    SET
        [ShippingDeliveryAreaId] =
            [address].[DeliveryAreaId],

        [ShippingRecipientName] =
            LEFT(
                [address].[RecipientName],
                200),

        [ShippingRecipientPhone] =
            LEFT(
                [address].[RecipientPhone],
                30),

        [ShippingCity] =
            LEFT(
                [address].[City],
                100),

        [ShippingAreaName] =
            LEFT(
                [address].[AreaName],
                200),

        [ShippingDetailedAddress] =
            LEFT(
                [address].[DetailedAddress],
                500),

        [ShippingBuildingNumber] =
            LEFT(
                [address].[BuildingNumber],
                50),

        [ShippingFloorNumber] =
            LEFT(
                [address].[FloorNumber],
                50),

        [ShippingApartmentNumber] =
            LEFT(
                [address].[ApartmentNumber],
                50),

        [ShippingLandmark] =
            LEFT(
                [address].[Landmark],
                300),

        [ShippingDeliveryAreaNameAr] =
            [area].[NameArabic],

        [ShippingDeliveryAreaNameEn] =
            [area].[NameEnglish]
    FROM [Orders] AS [order]
    INNER JOIN [UserAddresses] AS [address]
        ON [address].[Id] =
            [order].[UserAddressId]
    INNER JOIN [DeliveryAreas] AS [area]
        ON [area].[Id] =
            [address].[DeliveryAreaId]
    WHERE [order].[UserAddressId] IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM [UserAddresses]
        WHERE LEN([Label]) > 100
           OR LEN([City]) > 100
           OR LEN([AreaName]) > 200
           OR LEN([DetailedAddress]) > 500
           OR LEN([RecipientName]) > 200
           OR LEN([RecipientPhone]) > 30
           OR LEN([BuildingNumber]) > 50
           OR LEN([FloorNumber]) > 50
           OR LEN([ApartmentNumber]) > 50
           OR LEN([Landmark]) > 300
    )
    BEGIN
        THROW 51001,
            'Cannot migrate UserAddresses because one or more values exceed the new maximum lengths.',
            1;
    END;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    UPDATE [UserAddresses]
    SET [IsDefault] = 0
    WHERE [IsDefault] = 1
      AND [IsActive] = 0;

    ;WITH [RankedDefaults] AS
    (
        SELECT
            [Id],
            [IsDefault],
            ROW_NUMBER() OVER
            (
                PARTITION BY [UserId]
                ORDER BY [Id]
            ) AS [DefaultRank]
        FROM [UserAddresses]
        WHERE [IsDefault] = 1
          AND [IsActive] = 1
    )
    UPDATE [RankedDefaults]
    SET [IsDefault] = 0
    WHERE [DefaultRank] > 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'CustomAreaName');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [UserAddresses] DROP COLUMN [CustomAreaName];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'RequiresAreaReview');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [UserAddresses] DROP COLUMN [RequiresAreaReview];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'RecipientPhone');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [RecipientPhone] nvarchar(30) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'RecipientName');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [RecipientName] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'Landmark');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [Landmark] nvarchar(300) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'Label');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [Label] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'IsDefault');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [UserAddresses] ADD DEFAULT CAST(0 AS bit) FOR [IsDefault];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'IsActive');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [UserAddresses] ADD DEFAULT CAST(1 AS bit) FOR [IsActive];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'FloorNumber');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [FloorNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'DetailedAddress');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [DetailedAddress] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DROP INDEX [IX_UserAddresses_DeliveryAreaId] ON [UserAddresses];
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'DeliveryAreaId');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [DeliveryAreaId] int NOT NULL;
    CREATE INDEX [IX_UserAddresses_DeliveryAreaId] ON [UserAddresses] ([DeliveryAreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'City');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [City] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'BuildingNumber');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [BuildingNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'AreaName');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [AreaName] nvarchar(200) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserAddresses]') AND [c].[name] = N'ApartmentNumber');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [UserAddresses] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [UserAddresses] ALTER COLUMN [ApartmentNumber] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Orders]') AND [c].[name] = N'PaymentMethod');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Orders] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [Orders] ALTER COLUMN [PaymentMethod] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderItems]') AND [c].[name] = N'ProductId');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [OrderItems] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [OrderItems] ALTER COLUMN [ProductId] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderItems]') AND [c].[name] = N'VariantSummaryAr');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [OrderItems] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [OrderItems] ALTER COLUMN [VariantSummaryAr] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrderItems]') AND [c].[name] = N'VariantSummaryEn');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [OrderItems] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [OrderItems] ALTER COLUMN [VariantSummaryEn] nvarchar(500) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DeliveryAreas]') AND [c].[name] = N'Status');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [DeliveryAreas] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [DeliveryAreas] ALTER COLUMN [Status] int NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_UserAddresses_DeliveryAreaId_IsActive] ON [UserAddresses] ([DeliveryAreaId], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_UserAddresses_UserId_IsActive] ON [UserAddresses] ([UserId], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_UserAddresses_OneActiveDefaultPerUser] ON [UserAddresses] ([UserId]) WHERE [IsDefault] = 1 AND [IsActive] = 1');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [UserAddresses] ADD CONSTRAINT [CK_UserAddresses_DefaultAddressMustBeActive] CHECK ([IsDefault] = 0 OR [IsActive] = 1)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_Orders_OrderStatus_OrderDate] ON [Orders] ([OrderStatus], [OrderDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_Orders_PaymentMethod] ON [Orders] ([PaymentMethod]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_Orders_ShippingDeliveryAreaId] ON [Orders] ([ShippingDeliveryAreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Orders_UserId_IdempotencyKey] ON [Orders] ([UserId], [IdempotencyKey]) WHERE [IdempotencyKey] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_Amounts_NonNegative] CHECK ([SubTotal] >= 0 AND [Discount] >= 0 AND [DeliveryFee] >= 0 AND [TotalAmount] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_DeliveryMethod] CHECK ([DeliveryMethod] IN (1, 2))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_Discount_NotGreaterThan_SubTotal] CHECK ([Discount] <= [SubTotal])');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_OrderSource] CHECK ([OrderSource] IN (1, 2))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_OrderStatus] CHECK ([OrderStatus] IN (1, 2, 3, 4, 5, 6, 7, 8, 9))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_PaymentMethod] CHECK ([PaymentMethod] IN (1, 2))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [Orders] ADD CONSTRAINT [CK_Orders_PaymentStatus] CHECK ([PaymentStatus] IN (1, 2, 3, 4))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [OrderItems] ADD CONSTRAINT [CK_OrderItems_Prices_NonNegative] CHECK ([UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TotalPrice] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [OrderItems] ADD CONSTRAINT [CK_OrderItems_Quantity_Positive] CHECK ([Quantity] > 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [OrderItems] ADD CONSTRAINT [CK_OrderItems_RefundedQuantity] CHECK ([RefundedQuantity] >= 0 AND [RefundedQuantity] <= [Quantity])');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_DeliveryAreas_Status] ON [DeliveryAreas] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    CREATE INDEX [IX_DeliveryAreas_Status_IsActive] ON [DeliveryAreas] ([Status], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [DeliveryAreas] ADD CONSTRAINT [CK_DeliveryAreas_DeliveryFee_NonNegative] CHECK ([DeliveryFee] >= 0)');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    EXEC(N'ALTER TABLE [DeliveryAreas] ADD CONSTRAINT [CK_DeliveryAreas_Status] CHECK ([Status] IN (1, 2, 3))');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [OrderItems] ADD CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_DeliveryAreas_ShippingDeliveryAreaId] FOREIGN KEY ([ShippingDeliveryAreaId]) REFERENCES [DeliveryAreas] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [UserAddresses] ADD CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    ALTER TABLE [UserAddresses] ADD CONSTRAINT [FK_UserAddresses_DeliveryAreas_DeliveryAreaId] FOREIGN KEY ([DeliveryAreaId]) REFERENCES [DeliveryAreas] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260724142133_AddCheckoutFoundation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260724142133_AddCheckoutFoundation', N'8.0.28');
END;
GO

COMMIT;
GO
