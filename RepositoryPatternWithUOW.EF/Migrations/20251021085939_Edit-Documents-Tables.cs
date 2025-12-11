using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditDocumentsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meals_RestaurantId_Name",
                table: "Meals");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Vats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TradeMarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NationalIds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealCategories",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MealCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfIssuance",
                table: "CommercialRegistrations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CommercialRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "83524a49-2ad7-4800-ab6b-4d7912c4833b",
                column: "LockoutEnabled",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCategory_CategoryId_RestaurantId",
                table: "RestaurantCategory",
                columns: new[] { "CategoryId", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RestaurantId",
                table: "Meals",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantCategory_CategoryId_RestaurantId",
                table: "RestaurantCategory");

            migrationBuilder.DropIndex(
                name: "IX_Meals_RestaurantId",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vats");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TradeMarks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "DateOfIssuance",
                table: "CommercialRegistrations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CommercialRegistrations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealCategories",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MealCategories",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "83524a49-2ad7-4800-ab6b-4d7912c4833b",
                column: "LockoutEnabled",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_Meals_RestaurantId_Name",
                table: "Meals",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);
        }
    }
}
