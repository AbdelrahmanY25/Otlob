using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditCartDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerMeal",
                table: "CartDetails",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "AddOnsPrice",
                table: "CartDetails",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemsPrice",
                table: "CartDetails",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MealPrice",
                table: "CartDetails",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MealdDeteils",
                table: "CartDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddOnsPrice",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "ItemsPrice",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "MealPrice",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "MealdDeteils",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "CartDetails",
                newName: "PricePerMeal");
        }
    }
}
