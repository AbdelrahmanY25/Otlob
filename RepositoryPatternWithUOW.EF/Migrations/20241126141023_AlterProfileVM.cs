using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlterProfileVM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ProfileVM",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ProfileVM",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "ProfileVM");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "ProfileVM");
        }
    }
}
