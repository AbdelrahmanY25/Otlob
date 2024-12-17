using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCartInOrderMealsInOrderModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Orders_Cart_CartId",
            //    table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Orders",
                newName: "CartInOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CartId",
                table: "Orders",
                newName: "IX_Orders_CartInOrderId");

            migrationBuilder.RenameColumn(
                name: "ResturantId",
                table: "OrderedMeals",
                newName: "RestaurantId");

            migrationBuilder.AddColumn<int>(
                name: "CartInOrderId",
                table: "OrderedMeals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartInOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResturantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartInOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartInOrder_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealsInOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    MealId = table.Column<int>(type: "int", nullable: false),
                    MealName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MealDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CartInOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealsInOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealsInOrder_CartInOrder_CartInOrderId",
                        column: x => x.CartInOrderId,
                        principalTable: "CartInOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealsInOrder_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderedMeals_CartInOrderId",
                table: "OrderedMeals",
                column: "CartInOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedMeals_RestaurantId",
                table: "OrderedMeals",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_CartInOrder_UserId",
                table: "CartInOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealsInOrder_CartInOrderId",
                table: "MealsInOrder",
                column: "CartInOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MealsInOrder_MealId",
                table: "MealsInOrder",
                column: "MealId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
                table: "OrderedMeals",
                column: "CartInOrderId",
                principalTable: "CartInOrder",
                principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_OrderedMeals_Restaurants_RestaurantId",
            //    table: "OrderedMeals",
            //    column: "RestaurantId",
            //    principalTable: "Restaurants",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CartInOrder_CartInOrderId",
                table: "Orders",
                column: "CartInOrderId",
                principalTable: "CartInOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
                table: "OrderedMeals");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_OrderedMeals_Restaurants_RestaurantId",
            //    table: "OrderedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CartInOrder_CartInOrderId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "MealsInOrder");

            migrationBuilder.DropTable(
                name: "CartInOrder");

            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_CartInOrderId",
                table: "OrderedMeals");

            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_RestaurantId",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "CartInOrderId",
                table: "OrderedMeals");

            migrationBuilder.RenameColumn(
                name: "CartInOrderId",
                table: "Orders",
                newName: "CartId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CartInOrderId",
                table: "Orders",
                newName: "IX_Orders_CartId");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "OrderedMeals",
                newName: "ResturantId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Orders_Cart_CartId",
            //    table: "Orders",
            //    column: "CartId",
            //    principalTable: "Cart",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
