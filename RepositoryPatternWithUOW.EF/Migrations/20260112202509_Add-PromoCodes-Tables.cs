using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoCodesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[SubPrice] + [ServiceFeePrice] + [DeliveryFee] - [DiscountAmount]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldComputedColumnSql: "[SubPrice] + [ServiceFeePrice] + [DeliveryFee]",
                oldStored: true);

            migrationBuilder.CreateTable(
                name: "PromoCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxTotalUsage = table.Column<int>(type: "int", nullable: true),
                    MaxUsagePerUser = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFirstOrderOnly = table.Column<bool>(type: "bit", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCode_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromoCode_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeUsage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoCodeId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DiscountApplied = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsage_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsage_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsage_PromoCode_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "PromoCode",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "Description", "DiscountType", "DiscountValue", "IsActive", "IsFirstOrderOnly", "MaxDiscountAmount", "MaxTotalUsage", "MaxUsagePerUser", "MinOrderAmount", "RestaurantId", "ValidFrom", "ValidTo" },
                values: new object[] { 1, "WELCOME25", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "83524a49-2ad7-4800-ab6b-4d7912c4833b", "Welcome discount! Get 25% off on your first order.", "Percentage", 25m, true, true, null, null, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromoCodeId",
                table: "Orders",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_Code",
                table: "PromoCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_CreatedByUserId",
                table: "PromoCode",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_IsActive",
                table: "PromoCode",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_RestaurantId",
                table: "PromoCode",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_ValidFrom_ValidTo",
                table: "PromoCode",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsage_OrderId",
                table: "PromoCodeUsage",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsage_PromoCodeId",
                table: "PromoCodeUsage",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsage_PromoCodeId_UserId",
                table: "PromoCodeUsage",
                columns: new[] { "PromoCodeId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsage_UserId",
                table: "PromoCodeUsage",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PromoCode_PromoCodeId",
                table: "Orders",
                column: "PromoCodeId",
                principalTable: "PromoCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PromoCode_PromoCodeId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PromoCodeUsage");

            migrationBuilder.DropTable(
                name: "PromoCode");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PromoCodeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PromoCodeId",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[SubPrice] + [ServiceFeePrice] + [DeliveryFee]",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldComputedColumnSql: "[SubPrice] + [ServiceFeePrice] + [DeliveryFee] - [DiscountAmount]",
                oldStored: true);
        }
    }
}
