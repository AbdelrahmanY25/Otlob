using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditAdminsMonthlyAnalyticsTablesAddToTalCancelledOrdersCountColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrdersCount",
                table: "RestaurantMonthlyAnalytics",
                newName: "CompletedOrdersCount");

            migrationBuilder.RenameColumn(
                name: "OrdersCount",
                table: "AdminMonthlyAnalytics",
                newName: "CompletedOrdersCount");

            migrationBuilder.AddColumn<int>(
                name: "CancelledOrdersCount",
                table: "RestaurantMonthlyAnalytics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CancelledOrdersCount",
                table: "AdminMonthlyAnalytics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "RestaurantMonthlyAnalytics",
                type: "decimal(12,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.90)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.05)",
                oldStored: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "AdminMonthlyAnalytics",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.10)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.05)",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledOrdersCount",
                table: "RestaurantMonthlyAnalytics");

            migrationBuilder.DropColumn(
                name: "CancelledOrdersCount",
                table: "AdminMonthlyAnalytics");

            migrationBuilder.RenameColumn(
                name: "CompletedOrdersCount",
                table: "RestaurantMonthlyAnalytics",
                newName: "OrdersCount");

            migrationBuilder.RenameColumn(
                name: "CompletedOrdersCount",
                table: "AdminMonthlyAnalytics",
                newName: "OrdersCount");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "RestaurantMonthlyAnalytics",
                type: "decimal(12,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.05)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.90)",
                oldStored: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "AdminMonthlyAnalytics",
                type: "decimal(10,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.05)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.10)",
                oldStored: true);
        }
    }
}
