using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditAdminsDailyAnalyticsTablesChangeRevenuPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "RestaurantDailyAnalytics",
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
                table: "AdminDailyAnalytics",
                type: "decimal(9,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.10)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.05)",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalOrdersRevenue",
                table: "RestaurantDailyAnalytics",
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
                table: "AdminDailyAnalytics",
                type: "decimal(9,2)",
                nullable: false,
                computedColumnSql: "([TotalOrdersSales] * 0.05)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,2)",
                oldComputedColumnSql: "([TotalOrdersSales] * 0.10)",
                oldStored: true);
        }
    }
}
