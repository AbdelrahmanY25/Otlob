using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsOnAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Addresses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FloorNumber",
                table: "Addresses",
                type: "int",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNumberOrName",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceType",
                table: "Addresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "Addresses",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "FloorNumber",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "HouseNumberOrName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PlaceType",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "Addresses");
        }
    }
}
