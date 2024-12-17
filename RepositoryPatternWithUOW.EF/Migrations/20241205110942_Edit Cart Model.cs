using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditCartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurants_restaurantId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals");

            migrationBuilder.RenameColumn(
                name: "restaurantId",
                table: "Cart",
                newName: "RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_restaurantId",
                table: "Cart",
                newName: "IX_Cart_RestaurantId");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "OrderedMeals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "Cart",
                newName: "restaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart",
                newName: "IX_Cart_restaurantId");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "OrderedMeals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurants_restaurantId",
                table: "Cart",
                column: "restaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
