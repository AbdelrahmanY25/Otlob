using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryPatternWithUOW.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAcctiveStatusEnumToResturantModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accecpted",
                table: "Restaurants");

            migrationBuilder.AddColumn<int>(
                name: "AcctiveStatus",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcctiveStatus",
                table: "Restaurants");

            migrationBuilder.AddColumn<bool>(
                name: "Accecpted",
                table: "Restaurants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
