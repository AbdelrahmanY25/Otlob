using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationReasonsForOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerCancelReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestaurantCancelReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCancelReason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantCancelReason",
                table: "Orders");
        }
    }
}
