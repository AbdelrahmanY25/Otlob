using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangePKTypeForMealAddonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MealAddOns",
                table: "MealAddOns");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MealAddOns");

            migrationBuilder.AddColumn<string>(
                name: "MealAddOnId",
                table: "MealAddOns",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealAddOns",
                table: "MealAddOns",
                column: "MealAddOnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MealAddOns",
                table: "MealAddOns");

            migrationBuilder.DropColumn(
                name: "MealAddOnId",
                table: "MealAddOns");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MealAddOns",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealAddOns",
                table: "MealAddOns",
                column: "Id");
        }
    }
}
