using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsInOrder_CartInOrder_CartInOrderId",
                table: "MealsInOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CartInOrder_CartInOrderId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CartInOrder");

            migrationBuilder.DropIndex(
                name: "IX_MealsInOrder_CartInOrderId",
                table: "MealsInOrder");

            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MealDescription",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "MealName",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "CartInOrderId",
                table: "MealsInOrder");

            migrationBuilder.DropColumn(
                name: "MealDescription",
                table: "MealsInOrder");

            migrationBuilder.DropColumn(
                name: "MealName",
                table: "MealsInOrder");

            migrationBuilder.RenameColumn(
                name: "CustomerAddres",
                table: "Orders",
                newName: "AddressId");

            migrationBuilder.RenameColumn(
                name: "CartInOrderId",
                table: "Orders",
                newName: "AddressId1");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CartInOrderId",
                table: "Orders",
                newName: "IX_Orders_AddressId1");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "MealsInOrder",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Cart",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                newName: "IX_Cart_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "Resturant_Id",
                table: "AspNetUsers",
                newName: "RestaurantId");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderedMeals",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "MealsInOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "MealsPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealsPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealsPriceHistories_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealsInOrder_OrderId",
                table: "MealsInOrder",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RestaurantId",
                table: "AspNetUsers",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_MealsPriceHistories_MealId",
                table: "MealsPriceHistories",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealsPriceHistories_StartDate",
                table: "MealsPriceHistories",
                column: "StartDate");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AspNetUsers_Restaurants_RestaurantId",
            //    table: "AspNetUsers",
            //    column: "RestaurantId",
            //    principalTable: "Restaurants",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_AspNetUsers_ApplicationUserId",
                table: "Cart",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealsInOrder_Orders_OrderId",
                table: "MealsInOrder",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_AddressId1",
                table: "Orders",
                column: "AddressId1",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AspNetUsers_Restaurants_RestaurantId",
            //    table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_ApplicationUserId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsInOrder_Orders_OrderId",
                table: "MealsInOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_AddressId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "MealsPriceHistories");

            migrationBuilder.DropIndex(
                name: "IX_MealsInOrder_OrderId",
                table: "MealsInOrder");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RestaurantId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "MealsInOrder");

            migrationBuilder.RenameColumn(
                name: "AddressId1",
                table: "Orders",
                newName: "CartInOrderId");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Orders",
                newName: "CustomerAddres");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_AddressId1",
                table: "Orders",
                newName: "IX_Orders_CartInOrderId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "MealsInOrder",
                newName: "RestaurantId");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Cart",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_ApplicationUserId",
                table: "Cart",
                newName: "IX_Cart_UserId");

            migrationBuilder.RenameColumn(
                name: "RestaurantId",
                table: "AspNetUsers",
                newName: "Resturant_Id");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MealDescription",
                table: "OrderedMeals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealName",
                table: "OrderedMeals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "OrderedMeals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CartInOrderId",
                table: "MealsInOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MealDescription",
                table: "MealsInOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealName",
                table: "MealsInOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_MealsInOrder_CartInOrderId",
                table: "MealsInOrder",
                column: "CartInOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CartInOrder_UserId",
                table: "CartInOrder",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealsInOrder_CartInOrder_CartInOrderId",
                table: "MealsInOrder",
                column: "CartInOrderId",
                principalTable: "CartInOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CartInOrder_CartInOrderId",
                table: "Orders",
                column: "CartInOrderId",
                principalTable: "CartInOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
