using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdTypeToStringForMealOptionGroubAndItemsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MealOptionItems",
                table: "MealOptionItems");

            migrationBuilder.DropIndex(
                name: "IX_MealOptionItems_OptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MealOptionGroups",
                table: "MealOptionGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MealOptionItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MealOptionGroups");

            migrationBuilder.AddColumn<string>(
                name: "MealOptionItemId",
                table: "MealOptionItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionGroupMealOptionGroupId",
                table: "MealOptionItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MealOptionGroupId",
                table: "MealOptionGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealOptionItems",
                table: "MealOptionItems",
                column: "MealOptionItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealOptionGroups",
                table: "MealOptionGroups",
                column: "MealOptionGroupId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MealOptionItems",
                table: "MealOptionItems");

            migrationBuilder.DropIndex(
                name: "IX_MealOptionItems_OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MealOptionGroups",
                table: "MealOptionGroups");

            migrationBuilder.DropColumn(
                name: "MealOptionItemId",
                table: "MealOptionItems");

            migrationBuilder.DropColumn(
                name: "OptionGroupMealOptionGroupId",
                table: "MealOptionItems");

            migrationBuilder.DropColumn(
                name: "MealOptionGroupId",
                table: "MealOptionGroups");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MealOptionItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MealOptionGroups",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealOptionItems",
                table: "MealOptionItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MealOptionGroups",
                table: "MealOptionGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MealOptionItems_OptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealOptionItems_MealOptionGroups_OptionGroupId",
                table: "MealOptionItems",
                column: "OptionGroupId",
                principalTable: "MealOptionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
