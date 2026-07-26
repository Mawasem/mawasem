using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mawasem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: true),
                    ActorType = table.Column<int>(type: "int", nullable: false),
                    ChangedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistories", x => x.Id);
                    table.CheckConstraint("CK_OrderStatusHistories_ActorType", "[ActorType] IN (1, 2, 3)");
                    table.CheckConstraint("CK_OrderStatusHistories_ActorUser", "([ActorType] IN (1, 2) AND [ChangedByUserId] IS NOT NULL) OR ([ActorType] = 3 AND [ChangedByUserId] IS NULL)");
                    table.CheckConstraint("CK_OrderStatusHistories_NewStatus", "[NewStatus] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");
                    table.CheckConstraint("CK_OrderStatusHistories_PreviousStatus", "[PreviousStatus] IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");
                    table.CheckConstraint("CK_OrderStatusHistories_StatusChanged", "[PreviousStatus] <> [NewStatus]");
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ActorType",
                table: "OrderStatusHistories",
                column: "ActorType");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedAtUtc",
                table: "OrderStatusHistories",
                column: "ChangedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedByUserId",
                table: "OrderStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedByUserId_ActorType_ChangedAtUtc",
                table: "OrderStatusHistories",
                columns: new[] { "ChangedByUserId", "ActorType", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_NewStatus",
                table: "OrderStatusHistories",
                column: "NewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_NewStatus_ChangedAtUtc",
                table: "OrderStatusHistories",
                columns: new[] { "NewStatus", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId_ChangedAtUtc",
                table: "OrderStatusHistories",
                columns: new[] { "OrderId", "ChangedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderStatusHistories");
        }
    }
}
