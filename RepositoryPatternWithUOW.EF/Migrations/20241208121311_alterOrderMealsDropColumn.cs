using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class alterOrderMealsDropColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_CartInOrderId",
                table: "OrderedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
                table: "OrderedMeals");

            migrationBuilder.DropColumn(
                name: "CartInOrderId",
                table: "OrderedMeals");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
               name: "IX_OrderedMeals_CartInOrderId",
               table: "OrderedMeals",
               column: "CartInOrderId");

            migrationBuilder.AddForeignKey(
               name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
               table: "OrderedMeals",
               column: "CartInOrderId",
               principalTable: "CartInOrder",
               principalColumn: "Id");

            migrationBuilder.AddColumn<int>(
                name: "CartInOrderId",
                table: "OrderedMeals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
