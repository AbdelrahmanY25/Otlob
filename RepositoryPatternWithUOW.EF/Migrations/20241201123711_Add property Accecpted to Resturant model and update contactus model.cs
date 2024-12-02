using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddpropertyAccecptedtoResturantmodelandupdatecontactusmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "ContactUs",
                newName: "ResUserName");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ContactUs",
                newName: "ResPhone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ContactUs",
                newName: "ResName");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ContactUs",
                newName: "ResEmail");

            migrationBuilder.AddColumn<bool>(
                name: "Accecpted",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResAddress",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accecpted",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "ResAddress",
                table: "ContactUs");

            migrationBuilder.RenameColumn(
                name: "ResUserName",
                table: "ContactUs",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "ResPhone",
                table: "ContactUs",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ResName",
                table: "ContactUs",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "ResEmail",
                table: "ContactUs",
                newName: "Address");
            
        }
    }
}
