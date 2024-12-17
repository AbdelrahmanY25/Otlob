using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class DropFKInOrderedMealsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
            //    table: "OrderedMeals");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderedMeals_Restaurants_RestaurantId",
                table: "OrderedMeals");

            //migrationBuilder.DropIndex(
            //    name: "IX_OrderedMeals_CartInOrderId",
            //    table: "OrderedMeals");

            migrationBuilder.DropIndex(
                name: "IX_OrderedMeals_RestaurantId",
                table: "OrderedMeals");

            //migrationBuilder.DropColumn(
            //    name: "CartInOrderId",
            //    table: "OrderedMeals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "CartInOrderId",
            //    table: "OrderedMeals",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderedMeals_CartInOrderId",
            //    table: "OrderedMeals",
            //    column: "CartInOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedMeals_RestaurantId",
                table: "OrderedMeals",
                column: "RestaurantId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_OrderedMeals_CartInOrder_CartInOrderId",
            //    table: "OrderedMeals",
            //    column: "CartInOrderId",
            //    principalTable: "CartInOrder",
            //    principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderedMeals_Restaurants_RestaurantId",
                table: "OrderedMeals",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
