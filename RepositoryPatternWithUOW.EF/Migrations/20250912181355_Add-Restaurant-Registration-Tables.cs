using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantRegistrationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_ApplicationUserId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantBranches_RestaurantId",
                table: "RestaurantBranches");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Restaurants",
                newName: "OwnerRole");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Addresses",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_ApplicationUserId_CustomerAddress",
                table: "Addresses",
                newName: "IX_Addresses_UserId_CustomerAddress");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RestaurantBranches",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "RestaurantBranches",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "RestaurantBranches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "RestaurantBranches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "DeliveryRadiusKm",
                table: "RestaurantBranches",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "RestaurantBranches",
                type: "geography",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "RestaurantBranches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "RestaurantBranches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Addresses",
                type: "geography",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(29)", maxLength: 29, nullable: false),
                    BankCertificateImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankCertificateIssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommercialRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateRegistration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommercialRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommercialRegistrations_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialRegistrations_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommercialRegistrations_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NationalIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    NationalIdNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalIdExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FrontNationalIdImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackNationalIdImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NationalIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NationalIds_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NationalIds_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NationalIds_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    TrademarkName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrademarkNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TradeMarkCertificate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeMarks_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeMarks_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeMarks_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    VatNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VatCertificateImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vats_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vats_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vats_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantBranches_CreatedById",
                table: "RestaurantBranches",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantBranches_RestaurantId_Name",
                table: "RestaurantBranches",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantBranches_UpdatedById",
                table: "RestaurantBranches",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CreatedById",
                table: "BankAccounts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_RestaurantId",
                table: "BankAccounts",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_RestaurantId_AccountNumber",
                table: "BankAccounts",
                columns: new[] { "RestaurantId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_RestaurantId_Iban",
                table: "BankAccounts",
                columns: new[] { "RestaurantId", "Iban" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_UpdatedById",
                table: "BankAccounts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialRegistrations_CreatedById",
                table: "CommercialRegistrations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommercialRegistrations_RestaurantId",
                table: "CommercialRegistrations",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialRegistrations_RestaurantId_RegistrationNumber",
                table: "CommercialRegistrations",
                columns: new[] { "RestaurantId", "RegistrationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommercialRegistrations_UpdatedById",
                table: "CommercialRegistrations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CreatedById",
                table: "Contracts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_RestaurantId",
                table: "Contracts",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_UpdatedById",
                table: "Contracts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_CreatedById",
                table: "NationalIds",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_RestaurantId",
                table: "NationalIds",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_RestaurantId_NationalIdNumber",
                table: "NationalIds",
                columns: new[] { "RestaurantId", "NationalIdNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NationalIds_UpdatedById",
                table: "NationalIds",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMarks_CreatedById",
                table: "TradeMarks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMarks_RestaurantId",
                table: "TradeMarks",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeMarks_RestaurantId_TrademarkNumber",
                table: "TradeMarks",
                columns: new[] { "RestaurantId", "TrademarkNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeMarks_UpdatedById",
                table: "TradeMarks",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Vats_CreatedById",
                table: "Vats",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Vats_RestaurantId",
                table: "Vats",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vats_RestaurantId_VatNumber",
                table: "Vats",
                columns: new[] { "RestaurantId", "VatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vats_UpdatedById",
                table: "Vats",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantBranches_AspNetUsers_CreatedById",
                table: "RestaurantBranches",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantBranches_AspNetUsers_UpdatedById",
                table: "RestaurantBranches",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantBranches_AspNetUsers_CreatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantBranches_AspNetUsers_UpdatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "CommercialRegistrations");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "NationalIds");

            migrationBuilder.DropTable(
                name: "TradeMarks");

            migrationBuilder.DropTable(
                name: "Vats");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantBranches_CreatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantBranches_RestaurantId_Name",
                table: "RestaurantBranches");

            migrationBuilder.DropIndex(
                name: "IX_RestaurantBranches_UpdatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "City",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "DeliveryRadiusKm",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "RestaurantBranches");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "OwnerRole",
                table: "Restaurants",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Addresses",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_UserId_CustomerAddress",
                table: "Addresses",
                newName: "IX_Addresses_ApplicationUserId_CustomerAddress");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "RestaurantBranches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantBranches_RestaurantId",
                table: "RestaurantBranches",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_ApplicationUserId",
                table: "Addresses",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
