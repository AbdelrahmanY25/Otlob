using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Otlob.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvertisementFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertisementPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    AdvertisementPlanId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advertisements_AdvertisementPlans_AdvertisementPlanId",
                        column: x => x.AdvertisementPlanId,
                        principalTable: "AdvertisementPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advertisements_AspNetUsers_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advertisements_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementAnalytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertisementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Clicks = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementAnalytics_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdvertisementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "EGP"),
                    StripeSessionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StripeChargeId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardLast4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    CardBrand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StripeRefundId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementPayments_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvertisementPayments_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AdvertisementPlans",
                columns: new[] { "Id", "Description", "DescriptionAr", "DisplayOrder", "DurationInDays", "IsActive", "Name", "NameAr", "PricePerMonth" },
                values: new object[,]
                {
                    { 1, "Standard visibility for your restaurant. Your ad will appear in the restaurants list with a sponsored badge.", "ظهور عادي لمطعمك. سيظهر إعلانك في قائمة المطاعم مع شارة مُموَّل.", 3, 30, true, "Basic", "أساسي", 500m },
                    { 2, "Higher priority display. Your ad will appear at the top of the restaurants list and in the home page carousel.", "أولوية عرض أعلى. سيظهر إعلانك في أعلى قائمة المطاعم وفي شريط الصفحة الرئيسية.", 2, 30, true, "Premium", "مميز", 1000m },
                    { 3, "Top position with highlighted border. Your ad will be featured at the very top of all pages with special styling.", "أعلى موضع مع إطار مميز. سيتم عرض إعلانك في أعلى جميع الصفحات بتنسيق خاص.", 1, 30, true, "Featured", "متميز", 2000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementAnalytics_AdvertisementId",
                table: "AdvertisementAnalytics",
                column: "AdvertisementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPayments_AdvertisementId",
                table: "AdvertisementPayments",
                column: "AdvertisementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPayments_PaidAt",
                table: "AdvertisementPayments",
                column: "PaidAt");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPayments_PaymentStatus",
                table: "AdvertisementPayments",
                column: "PaymentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPayments_RestaurantId",
                table: "AdvertisementPayments",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPayments_StripeSessionId",
                table: "AdvertisementPayments",
                column: "StripeSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPlans_DisplayOrder",
                table: "AdvertisementPlans",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_AdvertisementPlanId",
                table: "Advertisements",
                column: "AdvertisementPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_EndDate",
                table: "Advertisements",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_RestaurantId",
                table: "Advertisements",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_ReviewedByUserId",
                table: "Advertisements",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_StartDate",
                table: "Advertisements",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Status",
                table: "Advertisements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_Status_StartDate_EndDate",
                table: "Advertisements",
                columns: new[] { "Status", "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementAnalytics");

            migrationBuilder.DropTable(
                name: "AdvertisementPayments");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "AdvertisementPlans");
        }
    }
}
