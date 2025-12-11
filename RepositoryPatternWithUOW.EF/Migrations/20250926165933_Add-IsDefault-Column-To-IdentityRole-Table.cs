using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDefaultColumnToIdentityRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId_CustomerAddress",
                table: "Addresses");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId_CustomerAddress_StreetName",
                table: "Addresses",
                columns: new[] { "UserId", "CustomerAddress", "StreetName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId_CustomerAddress_StreetName",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId_CustomerAddress",
                table: "Addresses",
                columns: new[] { "UserId", "CustomerAddress" },
                unique: true);
        }
    }
}
