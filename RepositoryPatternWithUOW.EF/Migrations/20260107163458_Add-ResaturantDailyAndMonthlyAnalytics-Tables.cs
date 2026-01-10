using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddResaturantDailyAndMonthlyAnalyticsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantDailyAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PendingOrders = table.Column<int>(type: "int", nullable: false),
                    PreparingOrders = table.Column<int>(type: "int", nullable: false),
                    ShippingOrders = table.Column<int>(type: "int", nullable: false),
                    DeliveredOrders = table.Column<int>(type: "int", nullable: false),
                    CancelledOrders = table.Column<int>(type: "int", nullable: false),
                    CompletedOrdersCount = table.Column<int>(type: "int", nullable: false),
                    TotalOrdersSales = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalOrdersRevenue = table.Column<decimal>(type: "decimal(12,2)", nullable: false, computedColumnSql: "([TotalOrdersSales] * 0.05)", stored: true),
                    AverageOrderPrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false, computedColumnSql: "CASE WHEN [CompletedOrdersCount] = 0 THEN 0 ELSE ([TotalOrdersSales] / [CompletedOrdersCount]) END", stored: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantDailyAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantDailyAnalytics_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantMonthlyAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    OrdersCount = table.Column<int>(type: "int", nullable: false),
                    TotalOrdersSales = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalOrdersRevenue = table.Column<decimal>(type: "decimal(12,2)", nullable: false, computedColumnSql: "([TotalOrdersSales] * 0.05)", stored: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantMonthlyAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantMonthlyAnalytics_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantDailyAnalytics_RestaurantId_Date",
                table: "RestaurantDailyAnalytics",
                columns: new[] { "RestaurantId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantMonthlyAnalytics_RestaurantId_Year_Month",
                table: "RestaurantMonthlyAnalytics",
                columns: new[] { "RestaurantId", "Year", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantDailyAnalytics");

            migrationBuilder.DropTable(
                name: "RestaurantMonthlyAnalytics");
        }
    }
}
