using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyRestaurantCategoryAndCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoryies",
                table: "Restaurants");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantCategory", x => new { x.CategoryId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_RestaurantCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantCategory_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Burger" },
                    { 2, "Shawarma" },
                    { 3, "Pizza" },
                    { 4, "FriedChicken" },
                    { 5, "EgyptionFood" },
                    { 6, "IndianFood" },
                    { 7, "ChineseFood" },
                    { 8, "JapaneseFood" },
                    { 9, "ItalianFood" },
                    { 10, "Sandwiches" },
                    { 11, "HealthyFood" },
                    { 12, "SeaFood" },
                    { 13, "Drinks" },
                    { 14, "IceCream" },
                    { 15, "Dessert" },
                    { 16, "Bakery" },
                    { 17, "Coffee" },
                    { 18, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantCategory_RestaurantId",
                table: "RestaurantCategory",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantCategory");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Categoryies",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
