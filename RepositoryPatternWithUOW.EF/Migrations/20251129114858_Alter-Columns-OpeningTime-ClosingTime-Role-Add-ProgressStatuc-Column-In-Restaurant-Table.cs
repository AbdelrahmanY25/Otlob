using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlterColumnsOpeningTimeClosingTimeRoleAddProgressStatucColumnInRestaurantTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "OwnerRole",
                table: "Restaurants",
                newName: "AdministratorRole");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "OpeningTime",
                table: "Restaurants",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ClosingTime",
                table: "Restaurants",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ProgressStatus",
                table: "Restaurants",
                type: "VARCHAR(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressStatus",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "AdministratorRole",
                table: "Restaurants",
                newName: "OwnerRole");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpeningTime",
                table: "Restaurants",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClosingTime",
                table: "Restaurants",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
