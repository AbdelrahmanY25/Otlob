using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOtpsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Otps",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredAt",
                table: "Otps",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "DATEADD(SECOND, 90, CreatedAt)",
                stored: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComputedColumnSql: "DATEADD(SECOND, 90, CreatedAt)",
                oldStored: null);

            migrationBuilder.CreateIndex(
                name: "IX_Otps_UserId",
                table: "Otps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Otps_AspNetUsers_UserId",
                table: "Otps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otps_AspNetUsers_UserId",
                table: "Otps");

            migrationBuilder.DropIndex(
                name: "IX_Otps_UserId",
                table: "Otps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Otps");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiredAt",
                table: "Otps",
                type: "datetime2",
                nullable: false,
                computedColumnSql: "DATEADD(SECOND, 90, CreatedAt)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComputedColumnSql: "DATEADD(SECOND, 90, CreatedAt)");
        }
    }
}
