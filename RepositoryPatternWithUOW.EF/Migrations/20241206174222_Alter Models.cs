using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlterModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerConcerns_Orders_OrderId",
                table: "CustomerConcerns");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Meals_MealId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MealId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerConcerns_OrderId",
                table: "CustomerConcerns");

            migrationBuilder.RenameColumn(
                name: "MealId",
                table: "Orders",
                newName: "ResturantId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "CustomerConcerns",
                newName: "ResturantId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CartId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "CustomerConcerns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CustomerConcerns",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Cart",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CartId",
                table: "Orders",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RestaurantId",
                table: "Orders",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedMeals_MealId",
                table: "OrderedMeals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerConcerns_RestaurantId",
                table: "CustomerConcerns",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerConcerns_UserId",
                table: "CustomerConcerns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerConcerns_AspNetUsers_UserId",
                table: "CustomerConcerns",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerConcerns_Restaurants_RestaurantId",
                table: "CustomerConcerns",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Meals_MealId",
                table: "OrderedMeals",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);          

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Restaurants_RestaurantId",
                table: "Orders",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurants_RestaurantId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerConcerns_AspNetUsers_UserId",
                table: "CustomerConcerns");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerConcerns_Restaurants_RestaurantId",
                table: "CustomerConcerns");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Meals_MealId",
                table: "OrderedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cart_CartId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Restaurants_RestaurantId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CartId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_RestaurantId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_MealId",
                table: "OrderedMeals");

            migrationBuilder.DropIndex(
                name: "IX_CustomerConcerns_RestaurantId",
                table: "CustomerConcerns");

            migrationBuilder.DropIndex(
                name: "IX_CustomerConcerns_UserId",
                table: "CustomerConcerns");

            migrationBuilder.DropIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "CustomerConcerns");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerConcerns");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "ResturantId",
                table: "Orders",
                newName: "MealId");

            migrationBuilder.RenameColumn(
                name: "ResturantId",
                table: "CustomerConcerns",
                newName: "OrderId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MealId",
                table: "Orders",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerConcerns_OrderId",
                table: "CustomerConcerns",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerConcerns_Orders_OrderId",
                table: "CustomerConcerns",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Meals_MealId",
                table: "Orders",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
