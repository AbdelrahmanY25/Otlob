using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminDailyAnalyticsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminDailyAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PendingOrders = table.Column<int>(type: "int", nullable: false),
                    PreparingOrders = table.Column<int>(type: "int", nullable: false),
                    ShippingOrders = table.Column<int>(type: "int", nullable: false),
                    DeliveredOrders = table.Column<int>(type: "int", nullable: false),
                    CancelledOrders = table.Column<int>(type: "int", nullable: false),
                    CompletedOrdersCount = table.Column<int>(type: "int", nullable: false),
                    TotalOrdersSales = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    TotalOrdersRevenue = table.Column<decimal>(type: "decimal(9,2)", nullable: false, computedColumnSql: "([TotalOrdersSales] * 0.05)", stored: true),
                    AverageOrderPrice = table.Column<decimal>(type: "decimal(9,2)", nullable: false, computedColumnSql: "CASE WHEN [CompletedOrdersCount] = 0 THEN 0 ELSE ([TotalOrdersSales] / [CompletedOrdersCount]) END", stored: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDailyAnalytics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminDailyAnalytics_Date",
                table: "AdminDailyAnalytics",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminDailyAnalytics");
        }
    }
}
