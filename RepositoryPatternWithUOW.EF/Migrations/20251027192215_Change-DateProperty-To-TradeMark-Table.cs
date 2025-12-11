using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDatePropertyToTradeMarkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TradeMarkCertificate",
                table: "TradeMarks");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpiryDate",
                table: "TradeMarks",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "TradeMarkCertificateId",
                table: "TradeMarks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMarks_TradeMarkCertificateId",
                table: "TradeMarks",
                column: "TradeMarkCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_TradeMarks_UploadedFiles_TradeMarkCertificateId",
                table: "TradeMarks",
                column: "TradeMarkCertificateId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradeMarks_UploadedFiles_TradeMarkCertificateId",
                table: "TradeMarks");

            migrationBuilder.DropIndex(
                name: "IX_TradeMarks_TradeMarkCertificateId",
                table: "TradeMarks");

            migrationBuilder.DropColumn(
                name: "TradeMarkCertificateId",
                table: "TradeMarks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "TradeMarks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "TradeMarkCertificate",
                table: "TradeMarks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
