using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mawasem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundRequestManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            // The previous refund tables have never contained production data.
            // Refuse to generate inaccurate financial snapshots if unexpected
            // legacy refund records exist in another environment.
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM [RefundRequests]
                )
                OR EXISTS (
                    SELECT 1
                    FROM [RefundRequestItems]
                )
                BEGIN
                    THROW 50001,
                        'AddRefundRequestManagement requires the existing refund tables to be empty.',
                        1;
                END;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_AspNetUsers_ReviewedByEmployeeId" ,
                table: "RefundRequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerReason" ,
                table: "RefundRequests" ,
                type: "nvarchar(1000)" ,
                maxLength: 1000 ,
                nullable: false ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(1000)" ,
                oldMaxLength: 1000 ,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt" ,
                table: "RefundRequests" ,
                type: "datetime2" ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedByEmployeeId" ,
                table: "RefundRequests" ,
                type: "int" ,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey" ,
                table: "RefundRequests" ,
                type: "nvarchar(100)" ,
                maxLength: 100 ,
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount" ,
                table: "RefundRequests" ,
                type: "decimal(18,2)" ,
                precision: 18 ,
                scale: 2 ,
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StockRestoredAtUtc" ,
                table: "RefundRequests" ,
                type: "datetime2" ,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestockQuantity" ,
                table: "RefundRequestItems" ,
                type: "int" ,
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedQuantity" ,
                table: "RefundRequestItems" ,
                type: "int" ,
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRefundAmount" ,
                table: "RefundRequestItems" ,
                type: "decimal(18,2)" ,
                precision: 18 ,
                scale: 2 ,
                nullable: false);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitRefundAmount" ,
                table: "RefundRequestItems" ,
                type: "decimal(18,2)" ,
                precision: 18 ,
                scale: 2 ,
                nullable: false);

            migrationBuilder.CreateTable(
                name: "RefundPaymentTransactions" ,
                columns: table => new
                {
                    Id = table.Column<int>(
                            type: "int" ,
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity" ,
                            "1, 1") ,

                    RefundRequestId = table.Column<int>(
                        type: "int" ,
                        nullable: false) ,

                    PaymentGateway = table.Column<int>(
                        type: "int" ,
                        nullable: false) ,

                    Status = table.Column<int>(
                        type: "int" ,
                        nullable: false) ,

                    Amount = table.Column<decimal>(
                        type: "decimal(18,2)" ,
                        precision: 18 ,
                        scale: 2 ,
                        nullable: false) ,

                    IdempotencyKey = table.Column<string>(
                        type: "nvarchar(100)" ,
                        maxLength: 100 ,
                        nullable: false) ,

                    ProviderTransactionId = table.Column<string>(
                        type: "nvarchar(200)" ,
                        maxLength: 200 ,
                        nullable: true) ,

                    ProviderReference = table.Column<string>(
                        type: "nvarchar(200)" ,
                        maxLength: 200 ,
                        nullable: true) ,

                    FailureCode = table.Column<string>(
                        type: "nvarchar(100)" ,
                        maxLength: 100 ,
                        nullable: true) ,

                    FailureMessage = table.Column<string>(
                        type: "nvarchar(2000)" ,
                        maxLength: 2000 ,
                        nullable: true) ,

                    RequestedAt = table.Column<DateTime>(
                        type: "datetime2" ,
                        nullable: false) ,

                    CompletedAt = table.Column<DateTime>(
                        type: "datetime2" ,
                        nullable: true) ,

                    InitiatedByEmployeeId = table.Column<int>(
                        type: "int" ,
                        nullable: true) ,

                    CompletedByEmployeeId = table.Column<int>(
                        type: "int" ,
                        nullable: true) ,

                    CreatedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset" ,
                        nullable: false) ,

                    CreatedBy = table.Column<string>(
                        type: "nvarchar(max)" ,
                        nullable: true) ,

                    LastModifiedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset" ,
                        nullable: true) ,

                    LastModifiedBy = table.Column<string>(
                        type: "nvarchar(max)" ,
                        nullable: true) ,

                    IsDeleted = table.Column<bool>(
                        type: "bit" ,
                        nullable: false) ,

                    DeletedOn = table.Column<DateTimeOffset>(
                        type: "datetimeoffset" ,
                        nullable: true) ,

                    DeletedBy = table.Column<string>(
                        type: "nvarchar(max)" ,
                        nullable: true)
                } ,
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_RefundPaymentTransactions" ,
                        transaction => transaction.Id);

                    table.CheckConstraint(
                        "CK_RefundPaymentTransactions_Amount_Positive" ,
                        "[Amount] > 0");

                    table.ForeignKey(
                        name:
                            "FK_RefundPaymentTransactions_AspNetUsers_CompletedByEmployeeId" ,
                        column: transaction =>
                            transaction.CompletedByEmployeeId ,
                        principalTable: "AspNetUsers" ,
                        principalColumn: "Id" ,
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name:
                            "FK_RefundPaymentTransactions_AspNetUsers_InitiatedByEmployeeId" ,
                        column: transaction =>
                            transaction.InitiatedByEmployeeId ,
                        principalTable: "AspNetUsers" ,
                        principalColumn: "Id" ,
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name:
                            "FK_RefundPaymentTransactions_RefundRequests_RefundRequestId" ,
                        column: transaction =>
                            transaction.RefundRequestId ,
                        principalTable: "RefundRequests" ,
                        principalColumn: "Id" ,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_CompletedAt" ,
                table: "RefundRequests" ,
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundRequests_CompletedByEmployeeId" ,
                table: "RefundRequests" ,
                column: "CompletedByEmployeeId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundRequests_OrderId_IdempotencyKey" ,
                table: "RefundRequests" ,
                columns: new[]
                {
                    "OrderId",
                    "IdempotencyKey"
                } ,
                unique: true);

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundRequests_StockRestoredAtUtc" ,
                table: "RefundRequests" ,
                column: "StockRestoredAtUtc");

            migrationBuilder.AddCheckConstraint(
                name:
                    "CK_RefundRequests_RefundAmount_NonNegative" ,
                table: "RefundRequests" ,
                sql: "[RefundAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name:
                    "CK_RefundRequestItems_RestockQuantity_Valid" ,
                table: "RefundRequestItems" ,
                sql:
                    "[RestockQuantity] >= 0 AND " +
                    "[RestockQuantity] <= [ReturnedQuantity]");

            migrationBuilder.AddCheckConstraint(
                name:
                    "CK_RefundRequestItems_ReturnedQuantity_Valid" ,
                table: "RefundRequestItems" ,
                sql:
                    "[ReturnedQuantity] >= 0 AND " +
                    "[ReturnedQuantity] <= [Quantity]");

            migrationBuilder.AddCheckConstraint(
                name:
                    "CK_RefundRequestItems_TotalRefundAmount_NonNegative" ,
                table: "RefundRequestItems" ,
                sql: "[TotalRefundAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name:
                    "CK_RefundRequestItems_UnitRefundAmount_NonNegative" ,
                table: "RefundRequestItems" ,
                sql: "[UnitRefundAmount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders" ,
                sql:
                    "[OrderStatus] IN " +
                    "(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders" ,
                sql:
                    "[PaymentStatus] IN " +
                    "(1, 2, 3, 4, 5)");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_CompletedByEmployeeId" ,
                table: "RefundPaymentTransactions" ,
                column: "CompletedByEmployeeId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_InitiatedByEmployeeId" ,
                table: "RefundPaymentTransactions" ,
                column: "InitiatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_PaymentGateway" ,
                table: "RefundPaymentTransactions" ,
                column: "PaymentGateway");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_ProviderReference" ,
                table: "RefundPaymentTransactions" ,
                column: "ProviderReference");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_ProviderTransactionId" ,
                table: "RefundPaymentTransactions" ,
                column: "ProviderTransactionId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_RefundRequestId" ,
                table: "RefundPaymentTransactions" ,
                column: "RefundRequestId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_RefundRequestId_IdempotencyKey" ,
                table: "RefundPaymentTransactions" ,
                columns: new[]
                {
                    "RefundRequestId",
                    "IdempotencyKey"
                } ,
                unique: true);

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_RefundRequestId_Status" ,
                table: "RefundPaymentTransactions" ,
                columns: new[]
                {
                    "RefundRequestId",
                    "Status"
                });

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_RequestedAt" ,
                table: "RefundPaymentTransactions" ,
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name:
                    "IX_RefundPaymentTransactions_Status" ,
                table: "RefundPaymentTransactions" ,
                column: "Status");

            migrationBuilder.AddForeignKey(
                name:
                    "FK_RefundRequests_AspNetUsers_CompletedByEmployeeId" ,
                table: "RefundRequests" ,
                column: "CompletedByEmployeeId" ,
                principalTable: "AspNetUsers" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name:
                    "FK_RefundRequests_AspNetUsers_ReviewedByEmployeeId" ,
                table: "RefundRequests" ,
                column: "ReviewedByEmployeeId" ,
                principalTable: "AspNetUsers" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropForeignKey(
                name:
                    "FK_RefundRequests_AspNetUsers_CompletedByEmployeeId" ,
                table: "RefundRequests");

            migrationBuilder.DropForeignKey(
                name:
                    "FK_RefundRequests_AspNetUsers_ReviewedByEmployeeId" ,
                table: "RefundRequests");

            migrationBuilder.DropTable(
                name: "RefundPaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RefundRequests_CompletedAt" ,
                table: "RefundRequests");

            migrationBuilder.DropIndex(
                name:
                    "IX_RefundRequests_CompletedByEmployeeId" ,
                table: "RefundRequests");

            migrationBuilder.DropIndex(
                name:
                    "IX_RefundRequests_OrderId_IdempotencyKey" ,
                table: "RefundRequests");

            migrationBuilder.DropIndex(
                name:
                    "IX_RefundRequests_StockRestoredAtUtc" ,
                table: "RefundRequests");

            migrationBuilder.DropCheckConstraint(
                name:
                    "CK_RefundRequests_RefundAmount_NonNegative" ,
                table: "RefundRequests");

            migrationBuilder.DropCheckConstraint(
                name:
                    "CK_RefundRequestItems_RestockQuantity_Valid" ,
                table: "RefundRequestItems");

            migrationBuilder.DropCheckConstraint(
                name:
                    "CK_RefundRequestItems_ReturnedQuantity_Valid" ,
                table: "RefundRequestItems");

            migrationBuilder.DropCheckConstraint(
                name:
                    "CK_RefundRequestItems_TotalRefundAmount_NonNegative" ,
                table: "RefundRequestItems");

            migrationBuilder.DropCheckConstraint(
                name:
                    "CK_RefundRequestItems_UnitRefundAmount_NonNegative" ,
                table: "RefundRequestItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CompletedAt" ,
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "CompletedByEmployeeId" ,
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey" ,
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "RefundAmount" ,
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "StockRestoredAtUtc" ,
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "RestockQuantity" ,
                table: "RefundRequestItems");

            migrationBuilder.DropColumn(
                name: "ReturnedQuantity" ,
                table: "RefundRequestItems");

            migrationBuilder.DropColumn(
                name: "TotalRefundAmount" ,
                table: "RefundRequestItems");

            migrationBuilder.DropColumn(
                name: "UnitRefundAmount" ,
                table: "RefundRequestItems");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerReason" ,
                table: "RefundRequests" ,
                type: "nvarchar(1000)" ,
                maxLength: 1000 ,
                nullable: true ,
                oldClrType: typeof(string) ,
                oldType: "nvarchar(1000)" ,
                oldMaxLength: 1000);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_OrderStatus" ,
                table: "Orders" ,
                sql:
                    "[OrderStatus] IN " +
                    "(1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_PaymentStatus" ,
                table: "Orders" ,
                sql:
                    "[PaymentStatus] IN " +
                    "(1, 2, 3, 4)");

            migrationBuilder.AddForeignKey(
                name:
                    "FK_RefundRequests_AspNetUsers_ReviewedByEmployeeId" ,
                table: "RefundRequests" ,
                column: "ReviewedByEmployeeId" ,
                principalTable: "AspNetUsers" ,
                principalColumn: "Id" ,
                onDelete: ReferentialAction.SetNull);
        }
    }
}