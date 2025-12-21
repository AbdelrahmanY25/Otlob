using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOptionGroupIdFKTypeToStringForOptionItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropIndex(
                name: "IX_MealOptionItems_OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropColumn(
                name: "OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.AlterColumn<string>(
                name: "OptionGroupId",
                table: "MealOptionItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_MealOptionItems_OptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupId",
                principalTable: "MealOptionGroups",
                principalColumn: "MealOptionGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropIndex(
                name: "IX_MealOptionItems_OptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.AlterColumn<int>(
                name: "OptionGroupId",
                table: "MealOptionItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "OptionGroupMealOptionGroupId",
                table: "MealOptionItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MealOptionItems_OptionGroupMealOptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupMealOptionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupMealOptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupMealOptionGroupId",
                principalTable: "MealOptionGroups",
                principalColumn: "MealOptionGroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
