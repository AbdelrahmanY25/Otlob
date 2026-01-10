using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminMonthlyAnalyticsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminMonthlyAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    OrdersCount = table.Column<int>(type: "int", nullable: false),
                    TotalOrdersSales = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalOrdersRevenue = table.Column<decimal>(type: "decimal(10,2)", nullable: false, computedColumnSql: "([TotalOrdersSales] * 0.05)", stored: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminMonthlyAnalytics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminMonthlyAnalytics_Year_Month",
                table: "AdminMonthlyAnalytics",
                columns: new[] { "Year", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminMonthlyAnalytics");
        }
    }
}
