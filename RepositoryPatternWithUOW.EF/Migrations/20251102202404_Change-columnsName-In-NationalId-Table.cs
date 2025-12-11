using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class ChangecolumnsNameInNationalIdTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_FrontNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropIndex(
                name: "IX_NationalIds_BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.RenameColumn(
                name: "FrontNationalCardId",
                table: "NationalIds",
                newName: "NationalCardId");

            migrationBuilder.RenameIndex(
                name: "IX_NationalIds_FrontNationalCardId",
                table: "NationalIds",
                newName: "IX_NationalIds_NationalCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIds_UploadedFiles_NationalCardId",
                table: "NationalIds",
                column: "NationalCardId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_NationalCardId",
                table: "NationalIds");

            migrationBuilder.RenameColumn(
                name: "NationalCardId",
                table: "NationalIds",
                newName: "FrontNationalCardId");

            migrationBuilder.RenameIndex(
                name: "IX_NationalIds_NationalCardId",
                table: "NationalIds",
                newName: "IX_NationalIds_FrontNationalCardId");

            migrationBuilder.AddColumn<string>(
                name: "BackNationalCardId",
                table: "NationalIds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_BackNationalCardId",
                table: "NationalIds",
                column: "BackNationalCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIds_UploadedFiles_BackNationalCardId",
                table: "NationalIds",
                column: "BackNationalCardId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIds_UploadedFiles_FrontNationalCardId",
                table: "NationalIds",
                column: "FrontNationalCardId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
