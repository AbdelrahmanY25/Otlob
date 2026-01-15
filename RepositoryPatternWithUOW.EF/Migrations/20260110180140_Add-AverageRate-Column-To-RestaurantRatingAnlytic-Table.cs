using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAverageRateColumnToRestaurantRatingAnlyticTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRate",
                table: "RestaurantRatingAnlytics",
                type: "decimal(18,0)",
                nullable: false,
                computedColumnSql: "CASE WHEN [RatesCount] = 0 THEN 0 ELSE CAST([Score] / [RatesCount] AS DECIMAL(12,2)) END",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRate",
                table: "RestaurantRatingAnlytics");
        }
    }
}
