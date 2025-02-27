using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class LinkCartToRestaurantModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
             name: "IX_Cart_RestaurantId",
             table: "Cart",
             column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart");
        }
    }
}
