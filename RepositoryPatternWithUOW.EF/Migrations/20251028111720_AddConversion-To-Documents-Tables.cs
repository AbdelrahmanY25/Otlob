using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddConversionToDocumentsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatCertificateImage",
                table: "Vats");

            migrationBuilder.DropColumn(
                name: "BackNationalIdImage",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "FrontNationalIdImage",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "SignatureImage",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "BankCertificateImage",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "VatNumber",
                table: "Vats",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Vats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "VatCertificateId",
                table: "Vats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TradeMarks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "NationalIds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BackNationalCardId",
                table: "NationalIds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrontNationalCardId",
                table: "NationalIds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignatureImageId",
                table: "NationalIds",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BankCertificateId",
                table: "BankAccounts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Vats_VatCertificateId",
                table: "Vats",
                column: "VatCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_BackNationalCardId",
                table: "NationalIds",
                column: "BackNationalCardId");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_FrontNationalCardId",
                table: "NationalIds",
                column: "FrontNationalCardId");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_SignatureImageId",
                table: "NationalIds",
                column: "SignatureImageId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BankCertificateId",
                table: "BankAccounts",
                column: "BankCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_UploadedFiles_BankCertificateId",
                table: "BankAccounts",
                column: "BankCertificateId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_NationalIds_UploadedFiles_SignatureImageId",
                table: "NationalIds",
                column: "SignatureImageId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vats_UploadedFiles_VatCertificateId",
                table: "Vats",
                column: "VatCertificateId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_UploadedFiles_BankCertificateId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_FrontNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropForeignKey(
                name: "FK_NationalIds_UploadedFiles_SignatureImageId",
                table: "NationalIds");

            migrationBuilder.DropForeignKey(
                name: "FK_Vats_UploadedFiles_VatCertificateId",
                table: "Vats");

            migrationBuilder.DropIndex(
                name: "IX_Vats_VatCertificateId",
                table: "Vats");

            migrationBuilder.DropIndex(
                name: "IX_NationalIds_BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropIndex(
                name: "IX_NationalIds_FrontNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropIndex(
                name: "IX_NationalIds_SignatureImageId",
                table: "NationalIds");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_BankCertificateId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "VatCertificateId",
                table: "Vats");

            migrationBuilder.DropColumn(
                name: "BackNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "FrontNationalCardId",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "SignatureImageId",
                table: "NationalIds");

            migrationBuilder.DropColumn(
                name: "BankCertificateId",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "VatNumber",
                table: "Vats",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Vats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "VatCertificateImage",
                table: "Vats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TradeMarks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "NationalIds",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BackNationalIdImage",
                table: "NationalIds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FrontNationalIdImage",
                table: "NationalIds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignatureImage",
                table: "NationalIds",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BankCertificateImage",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
