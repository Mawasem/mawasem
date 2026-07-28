using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mawasem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStorePosAndPickup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentMethod",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAtUtc",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ReturnNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefundPaymentMethod = table.Column<int>(type: "int", nullable: false),
                    RefundPaymentReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TotalRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReturnedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedByEmployeeId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReturns", x => x.Id);
                    table.CheckConstraint("CK_StoreReturns_PhysicalPaymentReference", "[RefundPaymentMethod] NOT IN (4, 5) OR ([RefundPaymentReference] IS NOT NULL AND LTRIM(RTRIM([RefundPaymentReference])) <> '')");
                    table.CheckConstraint("CK_StoreReturns_RefundPaymentMethod", "[RefundPaymentMethod] IN (3, 4, 5)");
                    table.CheckConstraint("CK_StoreReturns_TotalRefundAmount_NonNegative", "[TotalRefundAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_StoreReturns_AspNetUsers_ProcessedByEmployeeId",
                        column: x => x.ProcessedByEmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturns_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoreReturnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreReturnId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReturnItems", x => x.Id);
                    table.CheckConstraint("CK_StoreReturnItems_Amounts_NonNegative", "[UnitRefundAmount] >= 0 AND [TotalRefundAmount] >= 0");
                    table.CheckConstraint("CK_StoreReturnItems_Quantity_Positive", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_StoreReturnItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreReturnItems_StoreReturns_StoreReturnId",
                        column: x => x.StoreReturnId,
                        principalTable: "StoreReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_CustomerAssociation",
                table: "Orders",
                sql: "([OrderSource] = 1 AND [UserId] IS NOT NULL) OR ([OrderSource] = 2 AND [UserId] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentMethod",
                table: "Orders",
                sql: "[PaymentMethod] IN (1, 2, 3, 4, 5)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PhysicalPaymentReference",
                table: "Orders",
                sql: "[PaymentMethod] NOT IN (4, 5) OR ([PaymentReference] IS NOT NULL AND LTRIM(RTRIM([PaymentReference])) <> '')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_StoreSaleRules",
                table: "Orders",
                sql: "[OrderSource] <> 2 OR ([DeliveryMethod] = 2 AND [PaymentMethod] IN (3, 4, 5) AND [PaymentStatus] IN (2, 4, 5) AND [PaidAtUtc] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturnItems_OrderItemId",
                table: "StoreReturnItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturnItems_StoreReturnId",
                table: "StoreReturnItems",
                column: "StoreReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturnItems_StoreReturnId_OrderItemId",
                table: "StoreReturnItems",
                columns: new[] { "StoreReturnId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturns_OrderId",
                table: "StoreReturns",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturns_ProcessedByEmployeeId",
                table: "StoreReturns",
                column: "ProcessedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturns_ReturnedAtUtc",
                table: "StoreReturns",
                column: "ReturnedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReturns_ReturnNumber",
                table: "StoreReturns",
                column: "ReturnNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreReturnItems");

            migrationBuilder.DropTable(
                name: "StoreReturns");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_CustomerAssociation",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentMethod",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PhysicalPaymentReference",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_StoreSaleRules",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentMethod",
                table: "Orders",
                sql: "[PaymentMethod] IN (1, 2)");
        }
    }
}
