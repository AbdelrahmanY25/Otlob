using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnAddressAndMealTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meals_RestaurantId",
                table: "Meals");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ApplicationUserId",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RestaurantId_Name",
                table: "Meals",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId_CustomerAddress",
                table: "Addresses",
                columns: new[] { "ApplicationUserId", "CustomerAddress" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meals_RestaurantId_Name",
                table: "Meals");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ApplicationUserId_CustomerAddress",
                table: "Addresses");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RestaurantId",
                table: "Meals",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ApplicationUserId",
                table: "Addresses",
                column: "ApplicationUserId");
        }
    }
}
