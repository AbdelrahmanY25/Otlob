using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditOrderMealsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals");

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
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "OrderedMeals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Cart_CartId",
                table: "OrderedMeals",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");
        }
    }
}
