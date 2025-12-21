using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDisplayOrderNameToDisplayOrderForMealOptionGroubAndItemsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "MealOptionItems",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "MealOptionGroups",
                newName: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "MealOptionItems",
                newName: "DisplayOrder");

            migrationBuilder.RenameColumn(
                name: "DisplayOrder",
                table: "MealOptionGroups",
                newName: "DisplayOrder");
        }
    }
}
