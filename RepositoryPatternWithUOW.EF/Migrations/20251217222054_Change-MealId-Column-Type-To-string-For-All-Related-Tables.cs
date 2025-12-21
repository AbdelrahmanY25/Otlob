using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMealIdColumnTypeTostringForAllRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Meals_MealId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MealAddOns_Meals_MealId",
                table: "MealAddOns");

            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionGroups_Meals_MealId",
                table: "MealOptionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsPriceHistories_Meals_MealId",
                table: "MealsPriceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Meals_MealId",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meals",
                table: "Meals");

            migrationBuilder.DropIndex(
                name: "IX_Meals_Id_RestaurantId_CategoryId",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Meals");

            migrationBuilder.AlterColumn<string>(
                name: "MealId",
                table: "OrderDetails",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MealId",
                table: "MealsPriceHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MealId",
                table: "Meals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MealId",
                table: "MealOptionGroups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MealId",
                table: "MealAddOns",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MealId",
                table: "CartDetails",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meals",
                table: "Meals",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_MealId_RestaurantId_CategoryId",
                table: "Meals",
                columns: new[] { "MealId", "RestaurantId", "CategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Meals_MealId",
                table: "CartDetails",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealAddOns_Meals_MealId",
                table: "MealAddOns",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealOptionGroups_Meals_MealId",
                table: "MealOptionGroups",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealsPriceHistories_Meals_MealId",
                table: "MealsPriceHistories",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Meals_MealId",
                table: "OrderDetails",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "MealId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Meals_MealId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MealAddOns_Meals_MealId",
                table: "MealAddOns");

            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionGroups_Meals_MealId",
                table: "MealOptionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsPriceHistories_Meals_MealId",
                table: "MealsPriceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Meals_MealId",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meals",
                table: "Meals");

            migrationBuilder.DropIndex(
                name: "IX_Meals_MealId_RestaurantId_CategoryId",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "MealId",
                table: "Meals");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealsPriceHistories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Meals",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealOptionGroups",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealAddOns",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "CartDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meals",
                table: "Meals",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_Id_RestaurantId_CategoryId",
                table: "Meals",
                columns: new[] { "Id", "RestaurantId", "CategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Meals_MealId",
                table: "CartDetails",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealAddOns_Meals_MealId",
                table: "MealAddOns",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealOptionGroups_Meals_MealId",
                table: "MealOptionGroups",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MealsPriceHistories_Meals_MealId",
                table: "MealsPriceHistories",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Meals_MealId",
                table: "OrderDetails",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
