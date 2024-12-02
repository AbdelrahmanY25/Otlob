using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class EditResturantModellogocanbenull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descriptions",
                table: "Restaurants",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Restaurants",
                newName: "Descriptions");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
