using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMealIdColumnNameToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MealId",
                table: "Meals",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Meals_MealId_RestaurantId_CategoryId",
                table: "Meals",
                newName: "IX_Meals_Id_RestaurantId_CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Meals",
                newName: "MealId");

            migrationBuilder.RenameIndex(
                name: "IX_Meals_Id_RestaurantId_CategoryId",
                table: "Meals",
                newName: "IX_Meals_MealId_RestaurantId_CategoryId");
        }
    }
}
