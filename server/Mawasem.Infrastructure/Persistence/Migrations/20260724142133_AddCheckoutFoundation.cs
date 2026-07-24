using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mawasem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId" ,
                table: "UserAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_DeliveryAreas_DeliveryAreaId" ,
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_UserId" ,
                table: "UserAddresses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc" ,
                table: "Orders" ,
                type: "datetime2" ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey" ,
                table: "Orders" ,
                type: "nvarchar(128)" ,
                maxLength: 128 ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod" ,
                table: "Orders" ,
                type: "int" ,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAtUtc" ,
                table: "Orders" ,
                type: "datetime2" ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason" ,
                table: "Orders" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingApartmentNumber" ,
                table: "Orders" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAreaName" ,
                table: "Orders" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingBuildingNumber" ,
                table: "Orders" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity" ,
                table: "Orders" ,
                type: "nvarchar(100)" ,
                maxLength: 100 ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingDeliveryAreaId" ,
                table: "Orders" ,
                type: "int" ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingDeliveryAreaNameAr" ,
                table: "Orders" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingDeliveryAreaNameEn" ,
                table: "Orders" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingDetailedAddress" ,
                table: "Orders" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingFloorNumber" ,
                table: "Orders" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingLandmark" ,
                table: "Orders" ,
                type: "nvarchar(300)" ,
                maxLength: 300 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingRecipientName" ,
                table: "Orders" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingRecipientPhone" ,
                table: "Orders" ,
                type: "nvarchar(30)" ,
                maxLength: 30 ,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StockRestoredAtUtc" ,
                table: "Orders" ,
                type: "datetime2" ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId" ,
                table: "OrderItems" ,
                type: "int" ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantSummaryAr" ,
                table: "OrderItems" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantSummaryEn" ,
                table: "OrderItems" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status" ,
                table: "DeliveryAreas" ,
                type: "int" ,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [DeliveryAreas]
                SET [Status] = 2
                WHERE [Status] IS NULL;
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [PaymentMethod] = 1
                WHERE [PaymentMethod] IS NULL;
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.Sql(
                """
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
                """);

            migrationBuilder.DropColumn(
                name: "CustomAreaName" ,
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "RequiresAreaReview" ,
                table: "UserAddresses");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientPhone" ,
                table: "UserAddresses" ,
                type: "nvarchar(30)" ,
                maxLength: 30 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName" ,
                table: "UserAddresses" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Landmark" ,
                table: "UserAddresses" ,
                type: "nvarchar(300)" ,
                maxLength: 300 ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label" ,
                table: "UserAddresses" ,
                type: "nvarchar(100)" ,
                maxLength: 100 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault" ,
                table: "UserAddresses" ,
                type: "bit" ,
                nullable: false ,
                defaultValue: false ,
                oldClrType: typeof(bool) ,
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive" ,
                table: "UserAddresses" ,
                type: "bit" ,
                nullable: false ,
                defaultValue: true ,
                oldClrType: typeof(bool) ,
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FloorNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DetailedAddress" ,
                table: "UserAddresses" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryAreaId" ,
                table: "UserAddresses" ,
                type: "int" ,
                nullable: false ,
                oldClrType: typeof(int) ,
                oldType: "int" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City" ,
                table: "UserAddresses" ,
                type: "nvarchar(100)" ,
                maxLength: 100 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaName" ,
                table: "UserAddresses" ,
                type: "nvarchar(200)" ,
                maxLength: 200 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApartmentNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(50)" ,
                maxLength: 50 ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(max)" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethod" ,
                table: "Orders" ,
                type: "int" ,
                nullable: false ,
                oldClrType: typeof(int) ,
                oldType: "int" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId" ,
                table: "OrderItems" ,
                type: "int" ,
                nullable: false ,
                oldClrType: typeof(int) ,
                oldType: "int" ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VariantSummaryAr" ,
                table: "OrderItems" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(500)" ,
                oldMaxLength: 500 ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VariantSummaryEn" ,
                table: "OrderItems" ,
                type: "nvarchar(500)" ,
                maxLength: 500 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(500)" ,
                oldMaxLength: 500 ,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status" ,
                table: "DeliveryAreas" ,
                type: "int" ,
                nullable: false ,
                oldClrType: typeof(int) ,
                oldType: "int" ,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_DeliveryAreaId_IsActive" ,
                table: "UserAddresses" ,
                columns: new[] { "DeliveryAreaId" , "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId_IsActive" ,
                table: "UserAddresses" ,
                columns: new[] { "UserId" , "IsActive" });

            migrationBuilder.CreateIndex(
                name: "UX_UserAddresses_OneActiveDefaultPerUser" ,
                table: "UserAddresses" ,
                column: "UserId" ,
                unique: true ,
                filter: "[IsDefault] = 1 AND [IsActive] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserAddresses_DefaultAddressMustBeActive" ,
                table: "UserAddresses" ,
                sql: "[IsDefault] = 0 OR [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus_OrderDate" ,
                table: "Orders" ,
                columns: new[] { "OrderStatus" , "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethod" ,
                table: "Orders" ,
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingDeliveryAreaId" ,
                table: "Orders" ,
                column: "ShippingDeliveryAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_IdempotencyKey" ,
                table: "Orders" ,
                columns: new[] { "UserId" , "IdempotencyKey" } ,
                unique: true ,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Amounts_NonNegative" ,
                table: "Orders" ,
                sql: "[SubTotal] >= 0 AND [Discount] >= 0 AND [DeliveryFee] >= 0 AND [TotalAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_DeliveryMethod" ,
                table: "Orders" ,
                sql: "[DeliveryMethod] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Discount_NotGreaterThan_SubTotal" ,
                table: "Orders" ,
                sql: "[Discount] <= [SubTotal]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_OrderSource" ,
                table: "Orders" ,
                sql: "[OrderSource] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders" ,
                sql: "[OrderStatus] IN (1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentMethod" ,
                table: "Orders" ,
                sql: "[PaymentMethod] IN (1, 2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders" ,
                sql: "[PaymentStatus] IN (1, 2, 3, 4)");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId" ,
                table: "OrderItems" ,
                column: "ProductId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_Prices_NonNegative" ,
                table: "OrderItems" ,
                sql: "[UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TotalPrice] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_Quantity_Positive" ,
                table: "OrderItems" ,
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_RefundedQuantity" ,
                table: "OrderItems" ,
                sql: "[RefundedQuantity] >= 0 AND [RefundedQuantity] <= [Quantity]");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAreas_Status" ,
                table: "DeliveryAreas" ,
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAreas_Status_IsActive" ,
                table: "DeliveryAreas" ,
                columns: new[] { "Status" , "IsActive" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_DeliveryAreas_DeliveryFee_NonNegative" ,
                table: "DeliveryAreas" ,
                sql: "[DeliveryFee] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DeliveryAreas_Status" ,
                table: "DeliveryAreas" ,
                sql: "[Status] IN (1, 2, 3)");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId" ,
                table: "OrderItems" ,
                column: "ProductId" ,
                principalTable: "Products" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryAreas_ShippingDeliveryAreaId" ,
                table: "Orders" ,
                column: "ShippingDeliveryAreaId" ,
                principalTable: "DeliveryAreas" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId" ,
                table: "UserAddresses" ,
                column: "UserId" ,
                principalTable: "AspNetUsers" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_DeliveryAreas_DeliveryAreaId" ,
                table: "UserAddresses" ,
                column: "DeliveryAreaId" ,
                principalTable: "DeliveryAreas" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId" ,
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryAreas_ShippingDeliveryAreaId" ,
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId" ,
                table: "UserAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_DeliveryAreas_DeliveryAreaId" ,
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_DeliveryAreaId_IsActive" ,
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_UserId_IsActive" ,
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "UX_UserAddresses_OneActiveDefaultPerUser" ,
                table: "UserAddresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UserAddresses_DefaultAddressMustBeActive" ,
                table: "UserAddresses");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderStatus_OrderDate" ,
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethod" ,
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingDeliveryAreaId" ,
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId_IdempotencyKey" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Amounts_NonNegative" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_DeliveryMethod" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Discount_NotGreaterThan_SubTotal" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_OrderSource" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentMethod" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId" ,
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_Prices_NonNegative" ,
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_Quantity_Positive" ,
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_RefundedQuantity" ,
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAreas_Status" ,
                table: "DeliveryAreas");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAreas_Status_IsActive" ,
                table: "DeliveryAreas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DeliveryAreas_DeliveryFee_NonNegative" ,
                table: "DeliveryAreas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DeliveryAreas_Status" ,
                table: "DeliveryAreas");

            migrationBuilder.AddColumn<string>(
                name: "CustomAreaName" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAreaReview" ,
                table: "UserAddresses" ,
                type: "bit" ,
                nullable: false ,
                defaultValue: false);

            migrationBuilder.Sql(
                """
                UPDATE [address]
                SET
                    [CustomAreaName] =
                        CASE
                            WHEN [area].[Status] = 1
                                THEN [area].[NameEnglish]
                            ELSE NULL
                        END,

                    [RequiresAreaReview] =
                        CASE
                            WHEN [area].[Status] = 1
                                THEN 1
                            ELSE 0
                        END
                FROM [UserAddresses] AS [address]
                INNER JOIN [DeliveryAreas] AS [area]
                    ON [area].[Id] =
                        [address].[DeliveryAreaId];
                """);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientPhone" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(30)" ,
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(200)" ,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Landmark" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(300)" ,
                oldMaxLength: 300 ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(100)" ,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefault" ,
                table: "UserAddresses" ,
                type: "bit" ,
                nullable: false ,
                oldClrType: typeof(bool) ,
                oldType: "bit" ,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive" ,
                table: "UserAddresses" ,
                type: "bit" ,
                nullable: false ,
                oldClrType: typeof(bool) ,
                oldType: "bit" ,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "FloorNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(50)" ,
                oldMaxLength: 50 ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DetailedAddress" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(500)" ,
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryAreaId" ,
                table: "UserAddresses" ,
                type: "int" ,
                nullable: true ,
                oldClrType: typeof(int) ,
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "City" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(100)" ,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(50)" ,
                oldMaxLength: 50 ,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AreaName" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(200)" ,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ApartmentNumber" ,
                table: "UserAddresses" ,
                type: "nvarchar(max)" ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(50)" ,
                oldMaxLength: 50 ,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RejectedAtUtc" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RejectionReason" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingApartmentNumber" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAreaName" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingBuildingNumber" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingCity" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDeliveryAreaId" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDeliveryAreaNameAr" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDeliveryAreaNameEn" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDetailedAddress" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingFloorNumber" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingLandmark" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingRecipientName" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingRecipientPhone" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StockRestoredAtUtc" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId" ,
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantSummaryAr" ,
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantSummaryEn" ,
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Status" ,
                table: "DeliveryAreas");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId" ,
                table: "UserAddresses" ,
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId" ,
                table: "UserAddresses" ,
                column: "UserId" ,
                principalTable: "AspNetUsers" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_DeliveryAreas_DeliveryAreaId" ,
                table: "UserAddresses" ,
                column: "DeliveryAreaId" ,
                principalTable: "DeliveryAreas" ,
                principalColumn: "Id");
        }
    }
}