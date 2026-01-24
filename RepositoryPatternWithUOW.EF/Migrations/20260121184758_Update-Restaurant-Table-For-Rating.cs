using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRestaurantTableForRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantRatingAnlytics_RestaurantId",
                table: "RestaurantRatingAnlytics");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRatingAnlytics_RestaurantId",
                table: "RestaurantRatingAnlytics",
                column: "RestaurantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantRatingAnlytics_RestaurantId",
                table: "RestaurantRatingAnlytics");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantRatingAnlytics_RestaurantId",
                table: "RestaurantRatingAnlytics",
                column: "RestaurantId");
        }
    }
}
