using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class MakeTotalPriceCartDetailsTableColumnComputed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "CartDetails",
                type: "decimal(8,2)",
                nullable: false,
                computedColumnSql: "[Quantity] * ([MealPrice] + [ItemsPrice] + [AddOnsPrice])",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "CartDetails",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)",
                oldComputedColumnSql: "[Quantity] * ([MealPrice] + [ItemsPrice] + [AddOnsPrice])");
        }
    }
}
