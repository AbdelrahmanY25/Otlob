using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Orders_OrderId",
                table: "OrderedMeals");

            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_OrderId",
                table: "OrderedMeals");

            migrationBuilder.DropIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Cart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderedMeals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderedMeals_OrderId",
                table: "OrderedMeals",
                column: "OrderId");

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
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Orders_OrderId",
                table: "OrderedMeals",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
