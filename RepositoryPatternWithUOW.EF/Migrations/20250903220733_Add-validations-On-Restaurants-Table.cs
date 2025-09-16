using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddvalidationsOnRestaurantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_CreatedById",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_UpdatedById",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_CreatedById",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_UpdatedById",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "BussniesType",
                table: "Restaurants",
                newName: "Role");

            migrationBuilder.AddColumn<string>(
                name: "BusinessType",
                table: "Restaurants",
                type: "VARCHAR(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Restaurants",
                newName: "BussniesType");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Restaurants",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Restaurants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Restaurants",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Restaurants",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_CreatedById",
                table: "Restaurants",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_UpdatedById",
                table: "Restaurants",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_CreatedById",
                table: "Restaurants",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_UpdatedById",
                table: "Restaurants",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
