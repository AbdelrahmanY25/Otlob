using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantBranchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_UserId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Restaurants",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurants_UserId",
                table: "Restaurants",
                newName: "IX_Restaurants_OwnerId");

            migrationBuilder.AddColumn<int>(
                name: "BussniesType",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfBranches",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RestaurantBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MangerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MangerPhone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantBranches_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantBranches_RestaurantId",
                table: "RestaurantBranches",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants");

            migrationBuilder.DropTable(
                name: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "BussniesType",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "NumberOfBranches",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Restaurants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurants_OwnerId",
                table: "Restaurants",
                newName: "IX_Restaurants_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Restaurants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_UserId",
                table: "Restaurants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
